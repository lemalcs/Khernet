/* Definition for the DELETE_PARTIAL_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Delete all fragments of a message used when a text message is sent.
--
-- Parameters:
-- ID - The internal id of message.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE DELETE_PARTIAL_MESSAGE
(
  ID TYPE OF COLUMN PARTIAL_MESSAGE.ID
)
AS
BEGIN
   DELETE FROM PARTIAL_MESSAGE
   WHERE
   ID=:ID;
END