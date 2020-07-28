/* Definition for the GET_MESSAGES_HEADER procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-07-26
-- Autor: Luis Lema
--
-- Description: 
-- Get the header of the message.
--
-- Parameters:
-- ID_MESSAGE - The id of message.
--
-- Returns:
-- The header of the message
------------------------------------------------------------------------------

CREATE PROCEDURE GET_MESSAGES_HEADER
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
RETURNS
(
  ID TYPE OF COLUMN MESSAGE.ID,
  ID_SENDER TYPE OF COLUMN MESSAGE.ID_SENDER,
  CONTENT_TYPE TYPE OF COLUMN MESSAGE.CONTENT_TYPE,
  REG_DATE TIMESTAMP,
  STATE INTEGER,
  UID TYPE OF COLUMN MESSAGE.UID,
  TIMEID BIGINT
)
AS
BEGIN

   FOR SELECT ID,ID_SENDER,CONTENT_TYPE,REG_DATE,STATE,UID,TIMEID
   FROM MESSAGE
   WHERE
   ID=:ID_MESSAGE
   INTO :ID,:ID_SENDER,:CONTENT_TYPE,:REG_DATE,:STATE,:UID,:TIMEID
   DO
   SUSPEND;
   
END