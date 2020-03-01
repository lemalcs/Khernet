/* Definition for the REGISTER_NOT_SENT_MESSAGES procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Loads the list of messages that are pendding to be sent.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE REGISTER_NOT_SENT_MESSAGES
AS

DECLARE VARIABLE TODAY TIMESTAMP;

BEGIN
   TODAY=(SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE);

   --Remove messages with a state different than PENDING (0)
   DELETE FROM PENDDING_MESSAGE A 
   WHERE
   EXISTS(SELECT 1 FROM MESSAGE B
		WHERE
        A.ID_MESSAGE=B.ID
        AND B.STATE<>0);

   INSERT INTO PENDDING_MESSAGE(ID_RECEIPT,ID_MESSAGE,REG_DATE)
   SELECT ID_RECEIPT,ID,:TODAY FROM MESSAGE A
   WHERE
   ID_SENDER=0 --Current logged user
   AND STATE=0 --Not sent message
   AND NOT EXISTS(SELECT 1 FROM PENDDING_MESSAGE B
                   WHERE
                   A.ID_RECEIPT=B.ID_RECEIPT
                   AND A.ID=B.ID_MESSAGE);
END