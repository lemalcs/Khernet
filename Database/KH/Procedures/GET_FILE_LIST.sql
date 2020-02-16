/* Definition for the GET_FILE_LIST procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of files sent or received from a peer.
--
-- Parameters:
-- USER_TOKEN - The token of peer
-- FILE_TYPE - The type of file to search
--
-- Returns:
-- The list of id messages (numbers) which are the owners of files.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_FILE_LIST
(
  USER_TOKEN TYPE OF COLUMN PEER.TOKEN,
  FILE_TYPE TYPE OF COLUMN MESSAGE.CONTENT_TYPE
)
RETURNS
(
  ID INTEGER
)
AS
DECLARE VARIABLE USER_ID TYPE OF COLUMN PEER.ID;
BEGIN
   USER_ID=(SELECT ID FROM PEER WHERE TOKEN=:USER_TOKEN);
   
   FOR 
   SELECT ID
   FROM MESSAGE
   WHERE
   (ID_SENDER=:USER_ID OR ID_RECEIPT=:USER_ID)
   AND CONTENT_TYPE=:FILE_TYPE
   ORDER BY ID DESC
   INTO :ID
   DO
   SUSPEND;
END