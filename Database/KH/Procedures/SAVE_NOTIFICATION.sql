/* Definition for the SAVE_NOTIFICATION procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save a notification for later use.
--
-- Parameters:
-- TOKEN - The token of peer that fired the notification.
-- TYPE_NOT - The type of notification.
-- CONTENT - Summary of notification.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_NOTIFICATION
(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  TYPE_NOT TYPE OF COLUMN NOTIFICATION.TYPE_NOT,
  CONTENT TYPE OF COLUMN NOTIFICATION.CONTENT
)
AS

DECLARE VARIABLE TODAY TIMESTAMP;
DECLARE VARIABLE ID_TOKEN INTEGER;

BEGIN
   TODAY=(SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE);
   
   ID_TOKEN=(SELECT ID FROM PEER WHERE TOKEN=:TOKEN);

   INSERT INTO NOTIFICATION(ID_TOKEN,REG_DATE,TYPE_NOT,CONTENT,PROCESSED)
   VALUES(:ID_TOKEN,:TODAY,:TYPE_NOT,:CONTENT,'FALSE');
END