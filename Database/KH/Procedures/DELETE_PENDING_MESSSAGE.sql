/* Definition for the DELETE_PENDING_MESSSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Delete a pending message to be sent.
--
-- Parameters:
-- RECEIVER_TOKEN - The peer to sent message to.
-- ID_MESSAGE - The idof message to be sent.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE DELETE_PENDING_MESSSAGE
(
  RECEIVER_TOKEN TYPE OF COLUMN PEER.TOKEN,
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
AS
DECLARE VARIABLE ID_RECEIVER TYPE OF COLUMN PEER.ID;
BEGIN
   ID_RECEIVER=(SELECT ID FROM PEER WHERE TOKEN=:RECEIVER_TOKEN);

   DELETE FROM PENDING_MESSAGE 
   WHERE
   ID_RECEIVER=:ID_RECEIVER
   AND ID_MESSAGE=:ID_MESSAGE;
   
   UPDATE MESSAGE
   SET READ_STATE='TRUE'
   WHERE
   ID=:ID_MESSAGE;
   
END