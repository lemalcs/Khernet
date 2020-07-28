/* Definition for the GET_LAST_MESSAGES procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of sent and received messages with the peer.
--
-- Parameters:
-- PEER_TOKEN - The token of peer that sent or received the message.
-- LAST_ID_MESSAGE - The id of message from which to start searching messages.
-- FORWARD - The direction of searching, true to search and false to searck
--           backward.
-- QUANTITY - The quantity of message to load.
--
-- Returns:
-- The list of found messages.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_LAST_MESSAGES
(
  PEER_TOKEN TYPE OF COLUMN PEER.TOKEN,
  LAST_TIMEID_MESSAGE BIGINT,
  FORWARD BOOLEAN,
  QUANTITY INTEGER
)
RETURNS
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID,
  ID_SENDER INTEGER,
  CONTENT_TYPE TYPE OF COLUMN MESSAGE.CONTENT_TYPE,
  REG_DATE TIMESTAMP,
  STATE INTEGER,
  IS_READ BOOLEAN,
  UID TYPE OF COLUMN MESSAGE.UID,
  TIMEID BIGINT
)
AS

DECLARE VARIABLE ID_PEER INTEGER;
DECLARE VARIABLE ID_RECEIPT INTEGER;
DECLARE VARIABLE TIMEID_UNREAD_MESSAGE BIGINT;
BEGIN   
   ID_PEER=(SELECT ID FROM PEER WHERE TOKEN=:PEER_TOKEN);

   IF(LAST_TIMEID_MESSAGE>0) THEN
   BEGIN
      
      IF(FORWARD='FALSE') THEN
        FOR SELECT FIRST (:QUANTITY) 
        ID, ID_SENDER, CONTENT_TYPE,REG_DATE,STATE,READ_STATE,UID,TIMEID
        FROM MESSAGE 
        WHERE
        (ID_SENDER=:ID_PEER OR ID_RECEIPT=:ID_PEER)
        AND TIMEID<:LAST_TIMEID_MESSAGE
        AND NOT (ID_SENDER<>0 AND STATE<=0)
        ORDER BY TIMEID DESC
        INTO :ID_MESSAGE,:ID_SENDER,:CONTENT_TYPE,:REG_DATE,:STATE,:IS_READ,:UID,:TIMEID
        DO
        SUSPEND;
      ELSE
        FOR SELECT FIRST (:QUANTITY) 
        ID,ID_SENDER,CONTENT_TYPE,REG_DATE,STATE,READ_STATE,UID,TIMEID
        FROM MESSAGE
        WHERE
        (ID_SENDER=:ID_PEER OR ID_RECEIPT=:ID_PEER)
        AND TIMEID>:LAST_TIMEID_MESSAGE
        AND NOT (ID_SENDER<>0 AND STATE<=0)
        ORDER BY TIMEID ASC
        INTO :ID_MESSAGE,:ID_SENDER,:CONTENT_TYPE,:REG_DATE,:STATE,:IS_READ,:UID,:TIMEID
        DO
        SUSPEND;
      
    END
    ELSE
    BEGIN
         TIMEID_UNREAD_MESSAGE=(SELECT MIN(TIMEID) 
                      FROM MESSAGE 
                      WHERE 
                      READ_STATE='FALSE'
                      AND (ID_SENDER=:ID_PEER OR ID_RECEIPT=:ID_PEER)
                      AND NOT (ID_SENDER<>0 AND STATE<=0));
                      
         IF(TIMEID_UNREAD_MESSAGE IS NULL) THEN
           TIMEID_UNREAD_MESSAGE=0;
            
         IF(FORWARD='FALSE') THEN          
           FOR SELECT FIRST (:QUANTITY) 
           ID,ID_SENDER,CONTENT_TYPE,REG_DATE,STATE,READ_STATE,UID,TIMEID
           FROM MESSAGE
           WHERE
           (ID_SENDER=:ID_PEER OR ID_RECEIPT=:ID_PEER)
           AND TIMEID>=:TIMEID_UNREAD_MESSAGE
           AND NOT (ID_SENDER<>0 AND STATE<=0)
           ORDER BY TIMEID DESC
           INTO :ID_MESSAGE,:ID_SENDER,:CONTENT_TYPE,:REG_DATE,:STATE,:IS_READ,:UID,:TIMEID
           DO
           SUSPEND;
         ELSE
           FOR SELECT FIRST (:QUANTITY) 
           ID,ID_SENDER,CONTENT_TYPE,REG_DATE,STATE,READ_STATE,UID,TIMEID
           FROM MESSAGE
           WHERE
           (ID_SENDER=:ID_PEER OR ID_RECEIPT=:ID_PEER)
           AND TIMEID>=:TIMEID_UNREAD_MESSAGE
           AND NOT (ID_SENDER<>0 AND STATE<=0)
           ORDER BY TIMEID ASC
           INTO :ID_MESSAGE,:ID_SENDER,:CONTENT_TYPE,:REG_DATE,:STATE,:IS_READ,:UID,:TIMEID
           DO
           SUSPEND;
    END
END