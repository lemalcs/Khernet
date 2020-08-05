/* Definition for the REGISTER_NOT_SENT_MESSAGES procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Loads the list of messages that are pending to be sent.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE REGISTER_NOT_SENT_MESSAGES
AS
DECLARE VARIABLE TODAY TIMESTAMP;
BEGIN
   TODAY=(SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE);

   --Remove messages with a state different than PENDING (0)
   DELETE FROM PENDING_MESSAGE A 
   WHERE
   EXISTS(SELECT 1 FROM MESSAGE B
		WHERE
        A.ID_MESSAGE=B.ID
        AND B.STATE>0);

   --Pending file message to send to peer
   INSERT INTO PENDING_MESSAGE(ID_RECEIVER,ID_MESSAGE,REG_DATE)
   SELECT ID_RECEIVER,ID,:TODAY FROM MESSAGE A
   WHERE
   ID_SENDER=0 --Current logged user
   AND STATE=0 --Not sent message
   AND NOT EXISTS(SELECT 1 FROM PENDING_MESSAGE B
                   WHERE
                   A.ID_RECEIVER=B.ID_RECEIVER
                   AND A.ID=B.ID_MESSAGE);
                   
   --Pending file message to request to peer                
   INSERT INTO PENDING_MESSAGE(ID_RECEIVER,ID_MESSAGE,REG_DATE)
   SELECT ID_SENDER,ID,:TODAY FROM MESSAGE A
   WHERE
   ID_RECEIVER=0 --Current logged user
   AND STATE=-1 --Not downloaded file
   AND NOT EXISTS(SELECT 1 FROM PENDING_MESSAGE B
                   WHERE
                   A.ID_SENDER=B.ID_RECEIVER
                   AND A.ID=B.ID_MESSAGE);
END