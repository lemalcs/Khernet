/* Definition for the REGISTER_PENDING_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Register a message as pending to be sent.
--
-- Parameters:
-- RECEIVER_TOKEN - The token peer to send message.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE REGISTER_PENDING_MESSAGE
(
  RECEIVER_TOKEN TYPE OF COLUMN PEER.TOKEN,
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
AS

DECLARE VARIABLE TODAY TIMESTAMP;
DECLARE VARIABLE ID_RECEIVER TYPE OF COLUMN PEER.ID;

BEGIN
   TODAY=(SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE);
   
   ID_RECEIVER=(SELECT ID FROM PEER WHERE TOKEN=:RECEIVER_TOKEN);
   
   INSERT INTO PENDING_MESSAGE(ID_RECEIVER,ID_MESSAGE,REG_DATE)
   VALUES(:ID_RECEIVER,:ID_MESSAGE,:TODAY);
END