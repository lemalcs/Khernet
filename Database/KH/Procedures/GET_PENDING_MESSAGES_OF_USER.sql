/* Definition for the GET_PENDING_MESSAGES_OF_USER procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the pending messages to be sent to a peer.
--
-- Parameters:
-- RECEIVER_TOKEN - The token of peer.
-- QUANTITY - The number of message to obtain.
--
-- Returns:
-- The list of ids of message (numbers).
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PENDING_MESSAGES_OF_USER
(
  RECEIVER_TOKEN TYPE OF COLUMN PEER.TOKEN,
  QUANTITY INTEGER
)
RETURNS
(
  ID_MESSAGE INTEGER
)
AS
DECLARE VARIABLE RECEIVERID TYPE OF COLUMN PEER.TOKEN;
BEGIN
   RECEIVERID=(SELECT ID FROM PEER WHERE TOKEN=:RECEIVER_TOKEN);

    IF(QUANTITY<=0) THEN
    BEGIN
       FOR SELECT A.ID
       FROM MESSAGE A JOIN PEER B
       ON A.ID_RECEIVER=B.ID
       JOIN PENDING_MESSAGE C
       ON A.ID=C.ID_MESSAGE
       AND A.ID_RECEIVER=C.ID_RECEIVER
       WHERE
       B.ID=:RECEIVERID
	    AND A.STATE<1 --Filter processed and error message
       ORDER BY A.ID
       INTO :ID_MESSAGE
       DO
       SUSPEND;
    END
    ELSE
    BEGIN
       FOR SELECT FIRST(:QUANTITY) A.ID
       FROM MESSAGE A JOIN PEER B
       ON A.ID_RECEIVER=B.ID
       JOIN PENDING_MESSAGE C
       ON A.ID=C.ID_MESSAGE
       AND A.ID_RECEIVER=C.ID_RECEIVER
       WHERE
       B.ID=:RECEIVERID
	    AND A.STATE<1 --Filter processed and error message
       ORDER BY A.ID
       INTO :ID_MESSAGE
       DO
       SUSPEND;
    END
END