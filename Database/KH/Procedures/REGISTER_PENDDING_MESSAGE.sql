/* Definition for the REGISTER_PENDDING_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Register a message as pendding to be sent.
--
-- Parameters:
-- RECEIPT_TOKEN - The token peer to send message.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE REGISTER_PENDDING_MESSAGE
(
  RECEIPT_TOKEN TYPE OF COLUMN PEER.TOKEN,
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
AS

DECLARE VARIABLE TODAY TIMESTAMP;
DECLARE VARIABLE ID_RECEIPT TYPE OF COLUMN PEER.ID;

BEGIN
   TODAY=(SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE);
   
   ID_RECEIPT=(SELECT ID FROM PEER WHERE TOKEN=:RECEIPT_TOKEN);
   
   INSERT INTO PENDDING_MESSAGE(ID_RECEIPT,ID_MESSAGE,REG_DATE)
   VALUES(:ID_RECEIPT,:ID_MESSAGE,:TODAY);
END