using Khernet.Core.Entity;
using Khernet.Core.Processor;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using System;
using System.Collections.Generic;
using System.IO;

namespace Khernet.Core.Host
{
    public class Messenger
    {
        public List<Peer> GetPeers()
        {
            Communicator communicator = new Communicator();
            return communicator.GetPeers();
        }

        public Peer GetProfile()
        {
            try
            {
                Communicator communicator = new Communicator();
                return communicator.GetSelfProfile();
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public byte[] GetAvatar()
        {
            try
            {
                Communicator communicator = new Communicator();
                return communicator.GetSelfAvatar();
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public MessageProcessResult SendTextMessage(string senderToken, string receiverToken, byte[] message, int idReplyMessage, ContentType format, string uid, long timeId)
        {
            try
            {
                Communicator communicator = new Communicator();
                return communicator.SendTextMessage(senderToken, receiverToken, message, idReplyMessage, format, timeId, uid);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void SendWrtitingMessage(string senderToken, string receiverToken)
        {
            try
            {
                Communicator communicator = new Communicator();
                communicator.SendWritingMessage(senderToken, receiverToken);
            }
            catch (Exception exception)
            {
                //This event is not priority
                LogDumper.WriteLog(exception, "Writing message could not be sent.");
            }
        }

        public void MarkAsReadMessage(int idMessage)
        {
            try
            {
                Communicator communicator = new Communicator();
                communicator.MarkAsReadMessage(idMessage);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void BulkMarkAsReadMessage(int lastIdUnreadMessage)
        {
            try
            {
                Communicator communicator = new Communicator();
                communicator.BulkMarkAsReadMessage(lastIdUnreadMessage);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public Peer GetSelfProfile(string token)
        {
            Communicator communicator = new Communicator();
            return communicator.GetSelfProfile();
        }

        public Peer GetPeerProfile(string token)
        {
            try
            {
                Communicator communicator = new Communicator();
                return communicator.GetPeerProfile(token);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public byte[] GetPeerAvatar(string token)
        {
            try
            {
                Communicator communicator = new Communicator();
                return communicator.GetPeerAvatar(token);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void UpdatePeerProfile(string token)
        {
            try
            {
                Communicator communicator = new Communicator();
                communicator.UpdatePeerProfile(token);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void SavePeerDisplayname(string token, byte[] displayName)
        {
            try
            {
                Communicator communicator = new Communicator();
                communicator.SavePeerDisplayName(token, displayName);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void UpdatePeerAvatar(string token)
        {
            try
            {
                Communicator communicator = new Communicator();
                communicator.UpdatePeerAvatar(token);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void SaveProfile(Peer peer)
        {
            try
            {
                Communicator communicator = new Communicator();
                communicator.SaveProfile(peer);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void UpdateAvatar(byte[] avatar)
        {
            try
            {
                Communicator communicator = new Communicator();
                communicator.UpdateAvatar(avatar);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }
        public MessageProcessResult SendFile(FileObserver fileObserver)
        {
            try
            {
                FileCommunicator comm = new FileCommunicator();
                return comm.SendFile(fileObserver);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public Stream DownloadLocalFile(int idMessage)
        {
            FileCommunicator comm = new FileCommunicator();
            return comm.GetFile(idMessage);
        }

        public List<MessageItem> GetUnreadMessages(string senderToken)
        {
            Communicator comm = new Communicator();
            return comm.GetUnreadMessages(senderToken);
        }

        public MessageItem GetMessageHeader(int idMessage)
        {
            Communicator comm = new Communicator();
            return comm.GetMessageHeader(idMessage);
        }

        public byte[] GetMessageContent(int idMessage)
        {
            Communicator comm = new Communicator();
            return comm.GetMessageContent(idMessage);
        }

        public ChatMessage GetMessageDetail(int idMessage)
        {
            Communicator comm = new Communicator();
            return comm.GetMessageDetail(idMessage);
        }

        public byte[] GetThumbnail(int idMessage)
        {
            FileCommunicator comm = new FileCommunicator();
            return comm.GetThumbnail(idMessage);
        }

        public void SaveThumbnail(int idMessage, byte[] thumbnail)
        {
            FileCommunicator comm = new FileCommunicator();
            comm.SaveThumbnail(idMessage, thumbnail);
        }

        public List<MessageItem> GetLastMessages(string senderToken, bool forward, long lastTimeIdMessage, int quantity)
        {
            Communicator comm = new Communicator();
            return comm.GetLastMessages(senderToken, forward, lastTimeIdMessage, quantity);
        }

        public int SaveAnimation(int idMessage, string idFile, int width, int height, byte[] animation)
        {
            FileCommunicator fileData = new FileCommunicator();

            return fileData.SaveAnimation(idMessage, idFile, width, height, animation);
        }

        public string GetFileId(int idMessage)
        {
            FileCommunicator fileData = new FileCommunicator();
            return fileData.GetFileId(idMessage);
        }

        /// <summary>
        /// Get the numeric Id of chat message.
        /// </summary>
        /// <param name="uid">The UID of chat message.</param>
        /// <returns>The id of message.</returns>
        public int? GetIdMessage(string uid)
        {
            Communicator communicator = new Communicator();

            if (!string.IsNullOrEmpty(uid))
            {
                return communicator.GetIdReply(uid);
            }
            return null;
        }

        public long GetTimeIdMessage(int idMessage)
        {
            Communicator communicator = new Communicator();
            return communicator.GetTimeIdMessage(idMessage);
        }

        public List<int> GetAnimationList()
        {
            FileCommunicator fileData = new FileCommunicator();
            return fileData.GetAnimationList();
        }

        public AnimationDetail GetAnimationContent(int id)
        {
            FileCommunicator fileData = new FileCommunicator();
            return fileData.GetAnimationContent(id);
        }

        public string GetCacheFilePath(int idMessage)
        {
            FileCommunicator fileData = new FileCommunicator();
            return fileData.GetCacheFilePath(idMessage);
        }

        public void UpdateCacheFilePath(int idMessage, string filePath)
        {
            FileCommunicator fileData = new FileCommunicator();
            fileData.UpdateCacheFilePath(idMessage, filePath);
        }

        public List<MessageItem> GetFileList(string userToken, ContentType fileType, int lastIdMessage, int quantity)
        {
            FileCommunicator fileData = new FileCommunicator();
            return fileData.GetFileList(userToken, fileType, lastIdMessage, quantity);
        }

        public int GetFileCount(string userToken, ContentType fileType)
        {
            FileCommunicator fileData = new FileCommunicator();
            return fileData.GetFileCount(userToken, fileType);
        }
    }
}
