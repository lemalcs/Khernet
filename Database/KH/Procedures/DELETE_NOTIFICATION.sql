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
  ID TYPE OF COLUMN NOTIFICATION.ID
)
AS
DECLARE VARIABLE ID_TOKEN INTEGER;
BEGIN

   DELETE FROM NOTIFICATION
   WHERE
   ID=:ID;
   
END