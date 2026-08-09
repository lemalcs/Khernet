/* Definition for the GET_THUMBNAIL procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the thumbnail of a image or video.
--
-- Parameters:
-- ID_MESSAGE - The id of message which is the owner of thumbnail.
--
-- Returns:
-- An array of bytes containing the thumbnail (image).
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_THUMBNAIL
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
RETURNS
(
  THUMBNAIL TYPE OF COLUMN MESSAGE_FILE.THUMBN
)
AS
BEGIN
    FOR SELECT THUMBN FROM MESSAGE_FILE
    WHERE
    ID_MESSAGE=:ID_MESSAGE
    INTO :THUMBNAIL
    DO
    SUSPEND;
END