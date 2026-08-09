/* Definition for the GET_ANIMATION_CONTENT procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get a animation which is a GIF file converted to AVI file.
--
-- Parameters:
-- ID - The identifier of animation.
--
-- Returns:
-- A AVI format file.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_ANIMATION_CONTENT
(
  ID TYPE OF COLUMN ANIMATION.ID
)
RETURNS
(
  ID_MESSAGE INTEGER,
  ID_FILE TYPE OF COLUMN ANIMATION.ID_FILE,
  WIDTH TYPE OF COLUMN ANIMATION.WIDTH,
  HEIGHT TYPE OF COLUMN ANIMATION.HEIGHT,
  CONTENT BLOB
)
AS
BEGIN
   FOR 
   SELECT ID_MESSAGE,ID_FILE,WIDTH,HEIGHT,CONTENT
   FROM ANIMATION
   WHERE
   ID=:ID
   INTO :ID_MESSAGE,:ID_FILE,:WIDTH,:HEIGHT,:CONTENT
   DO
   SUSPEND;
END