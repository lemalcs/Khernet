using Khernet.Core.Common;
using Khernet.Core.Data;
using Khernet.Core.Entity;
using Khernet.Core.Processor.IoC;
using Khernet.Core.Processor.Managers;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Khernet.Core.Processor
{
    public class Communicator
    {
        public void ClearPartialMessages()
        {
            CommunicatorData commData = new CommunicatorData();
            commData.ClearPartialMessages();
        }

        public MessageProcessResult SendTextMessage(string senderToken, string receiptToken, byte[] message, int idReplyMessage, ContentType format, long timeId, string uid = null)
        {
            CommunicatorData commData = new CommunicatorData();

            if (string.IsNullOrEmpty(uid) || string.IsNullOrWhiteSpace(uid))
                uid = Guid.NewGuid().ToString().Replace("-", "");

            int idMessage = commData.SaveTextMessage(
                senderToken,
                receiptToken,
                DateTime.Now, message,
                (int)format,
                uid,
                timeId,
                idReplyMessage != 0 ? (int?)idReplyMessage : null);

            try
            {
                MessageProcessResult result = new MessageProcessResult(idMessage, MessageState.Pending);

                IoCContainer.Get<TextMessageManager>().ProcessMessage(idMessage);
                return result;
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                throw ex;
            }
        }

        private void SendMessageByChucks(InternalConversationMessage internalConversation, byte[] message)
        {
            try
            {
                CommunicatorClient commClient = new CommunicatorClient(internalConversation.ReceiptToken);

                int chukSize = 8192;

                int idChunk = 1;
                int totalChunks = message.Length / chukSize + (message.Length % chukSize > 0 ? 1 : 0);

                IEnumerable<byte> buffer = null;

                for (int i = 0; i < message.Length; i += chukSize)
                {
                    //Get next bytes to send
                    if (i + chukSize > message.Length)
                        buffer = message.Skip(i).Take(message.Length - i);
                    else
                        buffer = message.Skip(i).Take(chukSize);

                    ConversationMessage conversation = new ConversationMessage();

                    conversation.UID = internalConversation.UID;
                    conversation.TimeId = internalConversation.TimeId;
                    conversation.SenderToken = internalConversation.SenderToken;
                    conversation.ReceiptToken = internalConversation.ReceiptToken;
                    conversation.Sequential = idChunk;
                    conversation.RawContent = buffer.ToArray();
                    conversation.SendDate = internalConversation.SendDate;
                    conversation.TotalChunks = totalChunks;
                    conversation.UIDReply = internalConversation.UIDReply;
                    conversation.Type = internalConversation.Type;

                    //Send message to receipt
                    commClient.ProcessTextMessage(conversation);
                    idChunk++;
                }
            }
            catch (Exception ex)
            {
                LogDumper.WriteLog(ex);
                throw;
            }
        }

        public void RegisterPendingMessage(string receiptToken, int idMessage)
        {
            //Save file on database so this can be send later
            CommunicatorData commData = new CommunicatorData();
            commData.RegisterPendingMessage(receiptToken, idMessage);
        }

        public string GetUIDMessage(int idMessage)
        {
            CommunicatorData commData = new CommunicatorData();
            return commData.GetUIDMessage(idMessage);
        }

        public void SendPendingMessage(InternalConversationMessage conversation)
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable messageData = commData.GetMessageContent(conversation.Id);

            if (messageData.Rows.Count == 0)
                return;

            SendMessageByChucks(conversation, messageData.Rows[0][0] as byte[]);

            commData.DeletePendingMessage(conversation.ReceiptToken, conversation.Id);

            //Mark message as sent to receipt
            commData.SetMessageState(conversation.Id, (int)MessageState.Processed);

            IoCContainer.Get<NotificationManager>().ProcessMessageStateChanged(new MessageStateNotification
            {
                MessageId = conversation.Id,
                State = MessageState.Processed,
            });
        }

        public void SetMessageState(int idMessage, MessageState state)
        {
            CommunicatorData commData = new CommunicatorData();
            commData.SetMessageState(idMessage, (int)state);
        }

        public void SendWritingMessage(string senderToken, string receiptToken)
        {
            EventNotifierClient notifierClient = new EventNotifierClient(receiptToken);

            notifierClient.ProcessMessageProcessing(new MessageProcessingNotification
            {
                Process = MessageProcessing.WritingText,
                SenderToken = senderToken,
            });
        }

        public void ProcessTextMessage(ConversationMessage message)
        {
            CommunicatorData commData = new CommunicatorData();

            if (message.Sequential == message.TotalChunks)
            {
                DataTable chunksList = commData.GetPartialTextMessage(message.UID);

                List<byte> messageBytes = new List<byte>();
                byte[] buffer;
                for (int j = 0; j < chunksList.Rows.Count; j++)
                {
                    buffer = chunksList.Rows[j][1] as byte[];

                    for (int k = 0; k < buffer.Length; k++)
                    {
                        messageBytes.Add(buffer[k]);
                    }
                }

                chunksList.Rows.Clear();

                buffer = message.RawContent;

                for (int k = 0; k < buffer.Length; k++)
                {
                    messageBytes.Add(buffer[k]);
                }

                int? idReplyMessage = GetIdReply(message.UIDReply);

                buffer = messageBytes.ToArray();

                int idMessage = commData.SaveTextMessage(
                    message.SenderToken,
                    message.ReceiptToken,
                    message.SendDate,
                    buffer,
                    (int)message.Type,
                    message.UID,
                    message.TimeId,
                    idReplyMessage);

                commData.DeletePartialTextMessage(message.UID);

                commData.SetMessageState(idMessage, (int)MessageState.Processed);

                if (message.Type == ContentType.Text ||
                    message.Type == ContentType.Html ||
                    message.Type == ContentType.Markdown)
                {
                    PublisherClient publisherClient = new PublisherClient(Configuration.GetValue(Constants.PublisherService));

                    InternalConversationMessage conversation = new InternalConversationMessage();
                    conversation.SenderToken = message.SenderToken;
                    conversation.ReceiptToken = message.ReceiptToken;
                    conversation.SendDate = message.SendDate;
                    conversation.RawContent = buffer;
                    conversation.Id = idMessage;
                    conversation.IdReply = idReplyMessage.HasValue ? idReplyMessage.Value : 0;
                    conversation.Type = message.Type;

                    publisherClient.ProcessNewMessage(new MessageNotification
                    {
                        MessageId = idMessage,
                        SenderToken = message.SenderToken,
                        State = MessageState.Processed,
                        Format = message.Type,
                    });
                }
            }
            else
            {
                commData.SavePartialTextMessage(message.UID, message.Sequential, message.RawContent);
            }
        }

        public int? GetIdReply(string uid)
        {
            CommunicatorData commData = new CommunicatorData();

            if (!string.IsNullOrEmpty(uid))
            {
                return commData.GetIdMessage(uid);
            }
            return null;
        }

        public long GetTimeIdMessage(int idMessage)
        {
            CommunicatorData commData = new CommunicatorData();

            if (idMessage > 0)
            {
                return commData.GetTimeIdMessage(idMessage);
            }
            return 0;
        }

        public List<int> GetPendingMessageOfUser(string receiptToken, int quantity)
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable data = commData.GetPendingMessageOfUser(receiptToken, quantity);

            if (data.Rows.Count > 0)
            {
                List<int> messageIdList = new List<int>();

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    messageIdList.Add(Convert.ToInt32(data.Rows[i][0]));
                }
                return messageIdList;
            }
            else
                return null;
        }

        public List<int> GetRequestPendingMessageForUser(string senderToken, int quantity)
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable data = commData.GetRequestPendingMessageForUser(senderToken, quantity);

            if (data.Rows.Count > 0)
            {
                List<int> messageIdList = new List<int>();

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    messageIdList.Add(Convert.ToInt32(data.Rows[i][0]));
                }
                return messageIdList;
            }
            else
                return null;
        }

        public void DeletePendingMessage(string receiptToken, int idMessage)
        {
            CommunicatorData commData = new CommunicatorData();
            commData.DeletePendingMessage(receiptToken, idMessage);
        }

        public void MarkAsReadMessage(int idMessage)
        {
            CommunicatorData commData = new CommunicatorData();
            commData.MarkAsReadMessage(idMessage);
        }

        public void BulkMarkAsReadMessage(int lastIdUnreadMessage)
        {
            CommunicatorData commData = new CommunicatorData();
            commData.BulkMarkAsReadMessage(lastIdUnreadMessage);
        }

        public List<Peer> GetPeers()
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable dataList = commData.GetPeers();
            if (dataList.Rows.Count > 0)
            {
                List<Peer> peerList = new List<Peer>(dataList.Rows.Count);

                for (int i = 0; i < dataList.Rows.Count; i++)
                {
                    Peer p = new Peer();
                    p.UserName = dataList.Rows[i][0].ToString();
                    p.AccountToken = dataList.Rows[i][1].ToString();
                    p.FullName = dataList.Rows[i][2] as byte[];
                    p.DisplayName = dataList.Rows[i][3] as byte[];
                    p.HexColor = dataList.Rows[i][4].ToString();
                    p.Initials = dataList.Rows[i][5].ToString();
                    peerList.Add(p);
                }

                return peerList;
            }

            return null;
        }

        public string GetPeerAdress(string token, string serviceType)
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable dataList = commData.GetPeerAdress(token, serviceType);
            if (dataList.Rows.Count > 0)
            {
                return dataList.Rows[0][0].ToString();
            }

            return null;
        }

        public Peer GetSelfProfile()
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable data = commData.GetSelfProfile();

            if (data.Rows.Count > 0)
            {
                Peer account = new Peer();
                account.AccountToken = data.Rows[0][0].ToString();

                if (data.Rows[0][1] != DBNull.Value)
                    account.UserName = DecodeUserName(data.Rows[0][1].ToString());

                if (data.Rows[0][2] != DBNull.Value)
                    account.State = (PeerState)Convert.ToSByte(data.Rows[0][2]);

                if (data.Rows[0][3] != DBNull.Value)
                    account.Slogan = data.Rows[0][3].ToString();

                if (data.Rows[0][4] != DBNull.Value)
                    account.Group = data.Rows[0][4].ToString();

                if (data.Rows[0][5] != DBNull.Value)
                    account.FullName = data.Rows[0][5] as byte[];

                return account;
            }
            else
                return null;

        }

        private string DecodeUserName(string encodedName)
        {
            CryptographyProvider crypt = new CryptographyProvider();
            return Encoding.UTF8.GetString(crypt.DecodeBase58Check(encodedName));
        }

        public byte[] GetSelfAvatar()
        {
            CommunicatorData commData = new CommunicatorData();
            return commData.GetSelfAvatar();
        }

        public Peer GetPeerProfile(string token)
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable data = commData.GetPeerProfile(token);

            if (data.Rows.Count > 0)
            {
                Peer account = new Peer();
                account.AccountToken = token;

                if (data.Rows[0][0] != DBNull.Value)
                    account.UserName = data.Rows[0][0].ToString();

                if (data.Rows[0][1] != DBNull.Value)
                    account.State = (PeerState)Convert.ToSByte(data.Rows[0][1]);

                if (data.Rows[0][2] != DBNull.Value)
                    account.Slogan = data.Rows[0][2].ToString();

                if (data.Rows[0][3] != DBNull.Value)
                    account.Group = data.Rows[0][3].ToString();

                if (data.Rows[0][4] != DBNull.Value)
                    account.FullName = data.Rows[0][4] as byte[];

                if (data.Rows[0][5] != DBNull.Value)
                    account.DisplayName = data.Rows[0][5] as byte[];

                if (data.Rows[0][6] != DBNull.Value)
                    account.Initials = data.Rows[0][6].ToString();

                if (data.Rows[0][6] != DBNull.Value)
                    account.HexColor = data.Rows[0][7].ToString();

                return account;
            }
            else
            {
                throw new Exception("User not found"); ;
            }
        }

        public byte[] GetPeerAvatar(string token)
        {
            CommunicatorData commData = new CommunicatorData();
            byte[] avatar = commData.GetPeerAvatar(token);
            return avatar;
        }

        public bool VerifyUserExistence(string token)
        {
            return CommunicatorData.VerifyUserExistence(token);
        }


        public void SavePeer(string userName, string token, byte[] certificate, string address, string serviceType)
        {
            //Default hexadecimal color and initials
            string hexColor = null;
            string initials = null;

            //Check is user already exists in database
            if (!CommunicatorData.VerifyUserExistence(token))
            {
                hexColor = GenerateHexadecimalColor();
                initials = userName.Length > 1 ? userName.Substring(0, 2) : userName;
                initials = initials.ToUpper();
            }

            //Save the found user
            CommunicatorData.SavePeer(userName, token, certificate, address, serviceType, hexColor, initials);

            //Send a notification when a new peer is found
            if (serviceType == Constants.CommunicatorService || serviceType == Constants.FileService)
            {
                PublisherClient publisher = new PublisherClient(Configuration.GetValue(Constants.PublisherService));
                publisher.ProcessContactChange(new PeerNotification
                {
                    Token = token,
                    State = PeerState.New,
                    Change = serviceType == Constants.CommunicatorService ? PeerChangeType.ProfileChange : PeerChangeType.AvatarChange,
                });
            }
        }

        /// <summary>
        /// Generates a random color in hexadecimal format
        /// </summary>
        /// <returns>The hexadecimal vlue of color in format RRGGBB (Red, blue and green)</returns>
        private string GenerateHexadecimalColor()
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);

            //Generate values between 0 and 255
            int red = 0;
            int green = random.Next(0, 255);
            int blue = random.Next(0, 255);

            //Complete values that have one digit
            string redValue = (red < 16 ? "0" : "") + red.ToString("X");
            string greenValue = (green < 16 ? "0" : "") + green.ToString("X");
            string blueValue = (blue < 16 ? "0" : "") + blue.ToString("X");

            //Build hexadecimal color if format RRGGBB
            string color = $"{redValue}{greenValue}{blueValue}";

            return color;
        }

        public void SavePeerDisplayName(string token, byte[] displayName)
        {
            CommunicatorData d = new CommunicatorData();
            d.SavePeerDisplayName(token, displayName);
        }

        public void UpdatePeerState(string token, PeerState status)
        {
            CommunicatorData.UpdatePeerState(token, (short)status);

            PublisherClient publisher = new PublisherClient(Configuration.GetValue(Constants.PublisherService));

            //Send a notification of offline client to suscribers
            publisher.ProcessContactChange(new PeerNotification
            {
                Token = token,
                Change = PeerChangeType.StateChange,
                State = status
            });
        }

        public void ClearPeers()
        {
            CommunicatorData commData = new CommunicatorData();
            commData.ClearPeers();
        }

        public void ClearPeersState()
        {
            CommunicatorData commData = new CommunicatorData();
            commData.ClearPeersState();
        }

        public List<PeerAddress> GetConnectedPeers(string serviceType)
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable data = commData.GetConnectedPeers(serviceType);

            List<PeerAddress> addressList = new List<PeerAddress>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                addressList.Add(new PeerAddress(data.Rows[i][0].ToString(), data.Rows[i][1].ToString()));
            }

            return addressList;
        }

        public void RegisterNotSentMessage()
        {
            CommunicatorData commData = new CommunicatorData();
            commData.RegisterNotSentMessage();
        }

        public void UpdatePeerProfile(string token)
        {
            CommunicatorClient commClient = new CommunicatorClient(token);
            Peer peer = commClient.GetProfile();

            CommunicatorData.SavePeerProfile(token, peer.UserName, peer.Slogan, peer.Group, (sbyte)peer.State, peer.FullName);

        }

        public void UpdatePeerAvatar(string token)
        {
            CommunicatorData commData = new CommunicatorData();
            CommunicatorClient commClient = new CommunicatorClient(token);
            FileDownloadResponseMessage fileResponse = commClient.GetAvatar();

            if (fileResponse.File != null)
            {
                byte[] tempAvatar = new byte[2048];
                int read;
                using (MemoryStream fs = new MemoryStream())
                {
                    read = fileResponse.File.Read(tempAvatar, 0, 2048);
                    while (read > 0)
                    {
                        fs.Write(tempAvatar, 0, read);
                        read = fileResponse.File.Read(tempAvatar, 0, 2048);
                    }
                    tempAvatar = fs.ToArray();
                }

                commData.SavePeerAvatar(token, tempAvatar);
            }
        }

        public void UpdateAvatar(byte[] avatar)
        {
            CommunicatorData commData = new CommunicatorData();
            commData.SaveAvatar(avatar);
        }

        public void SaveProfile(Peer peer)
        {
            CommunicatorData commData = new CommunicatorData();
            AccountManager account = new AccountManager();
            commData.SaveProfile(account.EncodeUserName(peer.UserName), peer.Slogan, peer.Group, (sbyte)peer.State, peer.FullName);
        }

        public List<MessageItem> GetUnreadMessages(string senderToken)
        {
            CommunicatorData commData = new CommunicatorData();

            DataTable messagedata = commData.GetUnreadMessages(senderToken);

            if (messagedata.Rows.Count > 0)
            {
                List<MessageItem> messageList = new List<MessageItem>();

                for (int i = 0; i < messagedata.Rows.Count; i++)
                {
                    MessageItem message = new MessageItem
                    {
                        Id = Convert.ToInt32(messagedata.Rows[i][0]),
                        Format = (ContentType)Convert.ToInt32(messagedata.Rows[i][1]),
                        RegisterDate = Convert.ToDateTime(messagedata.Rows[i][2]),
                        State = Convert.ToInt32(messagedata.Rows[i][3]) == 1 ? MessageState.Processed : MessageState.Pending,
                        UID = messagedata.Rows[i][4].ToString(),
                        TimeId = Convert.ToInt64(messagedata.Rows[i][5]),
                    };

                    messageList.Add(message);
                }
                return messageList;
            }
            return null;
        }

        public MessageItem GetMessageHeader(int idMessage)
        {
            CommunicatorData commData = new CommunicatorData();

            DataTable messagedata = commData.GetMessageHeader(idMessage);

            if (messagedata.Rows.Count > 0)
            {
                MessageItem message = new MessageItem
                {
                    Id = Convert.ToInt32(messagedata.Rows[0][0]),
                    IdSenderPeer = Convert.ToInt32(messagedata.Rows[0][1]),
                    Format = (ContentType)Convert.ToInt32(messagedata.Rows[0][2]),
                    RegisterDate = Convert.ToDateTime(messagedata.Rows[0][3]),
                    State = Convert.ToInt32(messagedata.Rows[0][4]) == 1 ? MessageState.Processed : MessageState.Pending,
                    UID = messagedata.Rows[0][5].ToString(),
                    TimeId = Convert.ToInt64(messagedata.Rows[0][6]),
                };

                return message;
            }
            return null;
        }

        public List<MessageItem> GetLastMessages(string senderToken, bool forward, long lastTimeIdMessage, int quantity)
        {
            CommunicatorData commData = new CommunicatorData();

            DataTable messagedata = commData.GetLastMessages(senderToken, forward, lastTimeIdMessage, quantity);

            if (messagedata.Rows.Count > 0)
            {
                List<MessageItem> messageList = new List<MessageItem>();

                for (int i = 0; i < messagedata.Rows.Count; i++)
                {
                    MessageItem message = new MessageItem
                    {
                        Id = Convert.ToInt32(messagedata.Rows[i][0]),
                        IdSenderPeer = Convert.ToInt32(messagedata.Rows[i][1]),
                        Format = (ContentType)Convert.ToInt32(messagedata.Rows[i][2]),
                        RegisterDate = Convert.ToDateTime(messagedata.Rows[i][3]),
                        State = Convert.ToInt32(messagedata.Rows[i][4]) == 1 ? MessageState.Processed : MessageState.Pending,
                        IsRead = Convert.ToBoolean(messagedata.Rows[i][5]),
                        UID = messagedata.Rows[i][6] != DBNull.Value ? messagedata.Rows[i][6].ToString() : string.Empty,
                        TimeId = Convert.ToInt64(messagedata.Rows[i][7])
                    };

                    messageList.Add(message);
                }
                return messageList;
            }
            return null;
        }

        public byte[] GetMessageContent(int idMessage)
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable messageData = commData.GetMessageContent(idMessage);

            if (messageData.Rows.Count == 0)
                return null;

            return messageData.Rows[0][0] as byte[];

        }

        public ConversationMessage GetMessageDetail(int idMessage)
        {
            CommunicatorData commData = new CommunicatorData();
            DataTable messageData = commData.GetMessageDetail(idMessage);

            if (messageData.Rows.Count == 0)
                return null;

            InternalConversationMessage message = new InternalConversationMessage();
            message.SenderToken = messageData.Rows[0][0].ToString();
            message.ReceiptToken = messageData.Rows[0][1].ToString();
            message.RawContent = messageData.Rows[0][2] as byte[];
            message.TimeId = Convert.ToInt64(messageData.Rows[0][5]);
            message.SendDate = Convert.ToDateTime(messageData.Rows[0][6]);
            message.Id = idMessage;
            message.UID = messageData.Rows[0][4].ToString();

            if (messageData.Rows[0][7] != null && messageData.Rows[0][7] != DBNull.Value)
            {
                message.IdReply = Convert.ToInt32(messageData.Rows[0][7]);
                message.UIDReply = commData.GetUIDMessage(message.IdReply);
            }
            message.RawContent = messageData.Rows[0][2] as byte[];
            message.Type = (ContentType)Convert.ToInt32(messageData.Rows[0][3]);
            message.State = (MessageState)Convert.ToInt32(messageData.Rows[0][8]);

            return message;
        }
    }
}
