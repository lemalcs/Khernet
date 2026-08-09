/* Definition for the GET_MESSAGE_CONTENT procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the content of a message
--
-- Parameters:
-- ID_MESSAGE - The id of message.
--
-- Returns:
-- An array of bytes with content of message:
-- - HTML or Markdown for text messages.
-- - JSON for file messages.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_MESSAGE_CONTENT
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
RETURNS
(
  CONTENT BLOB,
  REG_DATE TIMESTAMP
)
AS
BEGIN
   FOR SELECT T.CONTENT,A.REG_DATE FROM MESSAGE A JOIN MESSAGE_TEXT T
   ON A.ID=T.ID_MESSAGE
   WHERE
   A.ID=:ID_MESSAGE
   INTO :CONTENT,:REG_DATE
   DO
   SUSPEND;  
END