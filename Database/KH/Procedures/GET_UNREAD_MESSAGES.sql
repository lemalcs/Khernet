/* Definition for the GET_UNREAD_MESSAGES procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of unread messages sent by a peer.
--
-- Parameters:
-- SENDER_TOKEN - The token of peer that sent the message.
--
-- Returns:
-- The list of messages.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_UNREAD_MESSAGES
(
  SENDER_TOKEN TYPE OF COLUMN PEER.TOKEN
)
RETURNS
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID,
  CONTENT_TYPE TYPE OF COLUMN MESSAGE.CONTENT_TYPE,
  REG_DATE TIMESTAMP,
  STATE INTEGER,
  UID TYPE OF COLUMN MESSAGE.UID,
  TIMEID BIGINT
)
AS

DECLARE VARIABLE ID_SENDER INTEGER;

BEGIN

   ID_SENDER=(SELECT ID FROM PEER WHERE TOKEN=:SENDER_TOKEN);

   FOR SELECT A.ID,A.CONTENT_TYPE,A.REG_DATE,A.STATE,UID,TIMEID
   FROM MESSAGE A 
   WHERE
   ID_SENDER=:ID_SENDER
   AND READ_STATE='FALSE'
   AND STATE=1
   ORDER BY A.REG_DATE ASC
   INTO :ID_MESSAGE,:CONTENT_TYPE,:REG_DATE,:STATE,:UID,:TIMEID
   DO
   SUSPEND;
END