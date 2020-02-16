/* Definition for the SAVE_ANIMATION procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save the animation which is a AVI file obtained from a GIF file.
--
-- Parameters:
-- ID_FILE - The internal id of file
-- WIDTH - The witdh of animation.
-- HEIGHT - The height of animation.
-- ANIMATION - The animation file (AVI format).
-- ID_MESSAGE - The id of message which is the owner of animation.
--
-- Returns:
-- The generated id of animation.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_ANIMATION
(
  ID_FILE TYPE OF COLUMN ANIMATION.ID_FILE,
  WIDTH TYPE OF COLUMN ANIMATION.WIDTH,
  HEIGHT TYPE OF COLUMN ANIMATION.HEIGHT,
  ANIMATION BLOB,
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
RETURNS
(
  ID_MSG TYPE OF COLUMN ANIMATION.ID
)
AS

DECLARE VARIABLE FILE_ID TYPE OF COLUMN ANIMATION.ID_FILE;

BEGIN
    FILE_ID=(SELECT ID_FILE FROM ANIMATION WHERE ID_FILE=:ID_FILE);

    IF(FILE_ID IS NULL)THEN
    BEGIN
       INSERT INTO ANIMATION(ID_FILE,WIDTH,HEIGHT,CONTENT,ID_MESSAGE)
       VALUES(:ID_FILE,:WIDTH,:HEIGHT,:ANIMATION,:ID_MESSAGE)
       RETURNING ID
       INTO :ID_MSG;
    END
    ELSE
        ID_MSG=-1;--Animation already exists
END