/* Definition for the SAVE_THUMBNAIL procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save the thumbnail of a image or video. 
--
-- Parameters:
-- ID_MESSAGE - The internal id of message that thumbanil belongs to.
-- THUMBNAIL - The thumbnail.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_THUMBNAIL
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID,
  THUMBNAIL TYPE OF COLUMN MESSAGE_FILE.THUMBN
)
AS
BEGIN
    INSERT INTO MESSAGE_FILE(ID_MESSAGE,THUMBN)
    VALUES(:ID_MESSAGE,:THUMBNAIL);
END