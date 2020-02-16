/* Definition for the SAVE_PARTIAL_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save a fragment of text message.
--
-- Parameters:
-- ID - The internal id of message.
-- SEQ - The numeric id that indicates sequence.
-- CONTENT - The content of message fragment.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_PARTIAL_MESSAGE
(
  ID TYPE OF COLUMN PARTIAL_MESSAGE.ID,
  SEQ TYPE OF COLUMN PARTIAL_MESSAGE.SEQ,
  CONTENT TYPE OF COLUMN PARTIAL_MESSAGE.CONTENT
)
AS
BEGIN
   INSERT INTO PARTIAL_MESSAGE(ID,SEQ,CONTENT)
   VALUES(:ID,:SEQ,:CONTENT);
END