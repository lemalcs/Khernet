/* Definition for the GET_REQUEST_PENDING_MESSAGES procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-03-09
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of incomplete file message to be requested to peer.
--
-- Parameters:
-- SENDER_TOKEN - The token of peer that sent file message.
-- QUANTITY - The quantity of message to load.
--
-- Returns:
-- The list of message ids.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_REQUEST_PENDING_MESSAGES(
  SENDER_TOKEN TYPE OF COLUMN PEER.TOKEN,
  QUANTITY INTEGER)
RETURNS(
  ID_MESSAGE INTEGER)
AS
DECLARE VARIABLE SENDERID TYPE OF COLUMN PEER.TOKEN;
BEGIN
   SENDERID=(SELECT ID FROM PEER WHERE TOKEN=:SENDER_TOKEN);

    IF(:QUANTITY<=0) THEN
    BEGIN
       FOR SELECT A.ID
       FROM MESSAGE A JOIN PENDING_MESSAGE B
       ON A.ID=B.ID_MESSAGE
       WHERE
       A.ID_SENDER=:SENDERID
       AND A.ID_SENDER=B.ID_RECEIVER
       AND A.ID_RECEIVER=0
       AND A.STATE<0
       ORDER BY A.ID
       INTO :ID_MESSAGE
       DO
       SUSPEND;
    END
    ELSE
    BEGIN
       FOR SELECT FIRST(:QUANTITY) A.ID
       FROM MESSAGE  A JOIN PENDING_MESSAGE B
       ON A.ID=B.ID_MESSAGE
       WHERE
       A.ID_SENDER=:SENDERID
       AND A.ID_SENDER=B.ID_RECEIVER
       AND A.ID_RECEIVER=0
       AND A.STATE<0
       ORDER BY A.ID
       INTO :ID_MESSAGE
       DO
       SUSPEND;
    END
END