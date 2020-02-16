/* Definition for the DELETE_PENDDING_MESSSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Delete a pendding message to be sent.
--
-- Parameters:
-- RECEIPT_TOKEN - The peer to sent message to.
-- ID_MESSAGE - The idof message to be sent.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE DELETE_PENDDING_MESSSAGE
(
  RECEIPT_TOKEN TYPE OF COLUMN PEER.TOKEN,
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
AS
DECLARE VARIABLE ID_RECEIPT TYPE OF COLUMN PEER.ID;
BEGIN
   ID_RECEIPT=(SELECT ID FROM PEER WHERE TOKEN=:RECEIPT_TOKEN);

   DELETE FROM PENDDING_MESSAGE 
   WHERE
   ID_RECEIPT=:ID_RECEIPT
   AND ID_MESSAGE=:ID_MESSAGE;
   
   UPDATE MESSAGE
   SET READ_STATE='TRUE'
   WHERE
   ID=:ID_MESSAGE;
   
END