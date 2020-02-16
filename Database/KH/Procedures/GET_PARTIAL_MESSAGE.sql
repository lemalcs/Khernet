/* Definition for the GET_PARTIAL_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of fragments of a messages.
--
-- Parameters:
-- ID - The internal id of message.
--
-- Returns:
-- The content of every fragment of message.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PARTIAL_MESSAGE
(
  ID TYPE OF COLUMN PARTIAL_MESSAGE.ID
)
RETURNS
(
  SEQ TYPE OF COLUMN PARTIAL_MESSAGE.SEQ,
  CONTENT TYPE OF COLUMN PARTIAL_MESSAGE.CONTENT
)
AS
BEGIN
   FOR SELECT SEQ,CONTENT FROM PARTIAL_MESSAGE
   WHERE
   ID=:ID
   ORDER BY SEQ
   INTO :SEQ,:CONTENT
   DO SUSPEND;
END