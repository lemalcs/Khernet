/* Definition for the SAVE_TEXT_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save a text message which is a header of any type of message.
--
-- Parameters:
-- SENDER_TOKEN - The sender of the message.
-- RECEIPT_TOKEN - The receiver of the message.
-- REG_DATE - The date that the message was created.
-- CONTENT - The detail of message.
-- CONTENT_TYPE - The type of message.
-- UID - The UID (Universal ID) of message.
-- ID_REPLY - The id of message being replied (It may not be set).
-- THUMBNAIL - The thumbnail of message if there is any.
-- FILE_PATH - The path of cache file (form file messages only)
--
-- Returns:
-- The internal id (number) of message.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_TEXT_MESSAGE
(
  SENDER_TOKEN TYPE OF COLUMN PEER.TOKEN,
  RECEIPT_TOKEN TYPE OF COLUMN PEER.TOKEN,
  REG_DATE TYPE OF COLUMN MESSAGE.REG_DATE,
  CONTENT TYPE OF COLUMN MESSAGE_TEXT.CONTENT,
  CONTENT_TYPE TYPE OF COLUMN MESSAGE.CONTENT_TYPE,
  UID TYPE OF COLUMN MESSAGE.UID,
  ID_REPLY TYPE OF COLUMN MESSAGE.ID_REPLY default null,
  THUMBNAIL BLOB default null,
  FILE_PATH BLOB DEFAULT NULL
)
RETURNS
(
  ID_MSG TYPE OF COLUMN MESSAGE.ID
)
AS

DECLARE VARIABLE TODAY TIMESTAMP;
DECLARE VARIABLE ID_SENDER_TOKEN INTEGER;
DECLARE VARIABLE ID_RECEIPT_TOKEN INTEGER;
DECLARE VARIABLE SELF_TOKEN TYPE OF COLUMN ACCOUNT.TOKEN;
DECLARE VARIABLE ID_MESSAGE INTEGER;
DECLARE VARIABLE IS_READ BOOLEAN;

BEGIN
    SELF_TOKEN=(SELECT TOKEN FROM ACCOUNT);
    
    IF(:SELF_TOKEN=:SENDER_TOKEN)THEN
    BEGIN
       ID_SENDER_TOKEN=0;
    END
    ELSE
    BEGIN
       ID_SENDER_TOKEN=(SELECT ID FROM PEER WHERE TOKEN=:SENDER_TOKEN);  
    END
    
    IF(:SELF_TOKEN=:RECEIPT_TOKEN)THEN
    BEGIN
       ID_RECEIPT_TOKEN=0;
    END
    ELSE
    BEGIN
       ID_RECEIPT_TOKEN=(SELECT ID FROM PEER WHERE TOKEN=:RECEIPT_TOKEN);  
    END
    
    TODAY=(SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE);
    
    --message is being send by current user
    IF(ID_SENDER_TOKEN=0) THEN
       IS_READ='TRUE';
    ELSE
       IS_READ='FALSE';
       
    --Save message header
    INSERT INTO MESSAGE(ID_SENDER,ID_RECEIPT,REG_DATE,READ_STATE,CONTENT_TYPE,UID,ID_REPLY)
    VALUES(:ID_SENDER_TOKEN,:ID_RECEIPT_TOKEN,:REG_DATE,:IS_READ,:CONTENT_TYPE,:UID,:ID_REPLY)
    RETURNING ID
    INTO :ID_MESSAGE;
    
    --Get generated id message
    ID_MSG=:ID_MESSAGE;
    
    --Save message content
    INSERT INTO MESSAGE_TEXT(ID_MESSAGE,CONTENT)
    VALUES(:ID_MSG,:CONTENT);
    
    --Check if message is a file
    IF(:CONTENT_TYPE!=6) THEN
    BEGIN
       INSERT INTO MESSAGE_FILE(ID_MESSAGE,THUMBN,FPATH)
       VALUES(:ID_MESSAGE,:THUMBNAIL,:FILE_PATH);
    END
    
    IF(:SELF_TOKEN<>:SENDER_TOKEN) THEN
    BEGIN
        INSERT INTO MESSAGE_NOTIFICATION(ID_SENDER,ID_MESSAGE,CONTENT_TYPE)
        VALUES(:ID_SENDER_TOKEN,:ID_MESSAGE,:CONTENT_TYPE);
    END
END