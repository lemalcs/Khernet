/* Definition for the DELETE_NOTIFICATION procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Delete a notification fired be a peer.
--
-- Parameters:
-- TOKEN - The token of the peer that fired the notification
-- TYPE_NOT - The type of notification to delete
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE DELETE_NOTIFICATION
(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  TYPE_NOT TYPE OF COLUMN NOTIFICATION.TYPE_NOT
)
AS
DECLARE VARIABLE ID_TOKEN INTEGER;
BEGIN
   ID_TOKEN=(SELECT ID FROM PEER WHERE TOKEN=:TOKEN);
   
   DELETE FROM NOTIFICATION
   WHERE
   ID_TOKEN=:ID_TOKEN
   AND TYPE_NOT=:TYPE_NOT
   ROWS 1;
END