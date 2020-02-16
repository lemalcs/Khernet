/* Definition for the GET_CACHE_FILE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the path of cache file.
--
-- Parameters:
-- ID_MESSAGE - The id of message wich is the owner of file
--
-- Returns:
-- A byte array containing the path of file.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_CACHE_FILE
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
RETURNS
(
  FILE_PATH BLOB
)
AS
BEGIN
    FOR SELECT FPATH FROM MESSAGE_FILE
    WHERE
    ID_MESSAGE=:ID_MESSAGE
    INTO :FILE_PATH
    DO
    SUSPEND;
END