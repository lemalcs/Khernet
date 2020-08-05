/* Definition for the GET_FILE_COUNT procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-04-19
-- Autor: Luis Lema
--
-- Description: 
-- Get number of files shared between current user and remote peer.
--
-- Parameters:
-- USER_TOKEN - The token of peer.
-- FILE_TYPE - The type of file to count.
--
-- Returns:
-- The number of files of given type.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_FILE_COUNT
(
  USER_TOKEN TYPE OF COLUMN PEER.TOKEN,
  FILE_TYPE TYPE OF COLUMN MESSAGE.CONTENT_TYPE
)
RETURNS
(
  QUANTITY INTEGER
)
AS
DECLARE VARIABLE USER_ID TYPE OF COLUMN PEER.ID;
BEGIN
   USER_ID=(SELECT ID FROM PEER WHERE TOKEN=:USER_TOKEN);

   FOR SELECT COUNT(1)
   FROM MESSAGE
   WHERE
   (ID_SENDER=:USER_ID OR ID_RECEIVER=:USER_ID)
   AND CONTENT_TYPE=:FILE_TYPE
   INTO :QUANTITY
   DO SUSPEND;
END