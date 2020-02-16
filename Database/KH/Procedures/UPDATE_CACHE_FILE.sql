/* Definition for the UPDATE_CACHE_FILE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Update the path (local file system) where cache file is located.
--
-- Parameters:
-- ID_MESSAGE - The id of message that the file belongs to.
-- FILE_PATH - The path of cache file
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE UPDATE_CACHE_FILE
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID,
  FILE_PATH BLOB
)
AS
BEGIN
    UPDATE OR INSERT INTO MESSAGE_FILE(ID_MESSAGE,FPATH)
    VALUES(:ID_MESSAGE,:FILE_PATH)
    MATCHING(ID_MESSAGE);
END