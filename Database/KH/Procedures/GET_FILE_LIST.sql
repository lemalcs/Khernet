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
-- LAST_ID_MESSAGE - The id of message from to start searching (not included in results)
-- QUANTITY - The number of messages to retrieve.
--
-- Returns:
-- The list of messages headers.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_FILE_LIST
(
  USER_TOKEN TYPE OF COLUMN PEER.TOKEN,
  FILE_TYPE TYPE OF COLUMN MESSAGE.CONTENT_TYPE,
  LAST_ID_MESSAGE INTEGER,
  QUANTITY INTEGER)
RETURNS
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID,
  ID_SENDER INTEGER,
  REG_DATE TIMESTAMP,
  STATE INTEGER,
  IS_READ BOOLEAN,
  UID TYPE OF COLUMN MESSAGE.UID,
  TIMEID BIGINT
)
AS
DECLARE VARIABLE USER_ID TYPE OF COLUMN PEER.ID;
BEGIN
   USER_ID=(SELECT ID FROM PEER WHERE TOKEN=:USER_TOKEN);
   
   IF(:LAST_ID_MESSAGE<=0 AND :QUANTITY<=0)THEN
	BEGIN
       FOR 
	   SELECT ID,ID_SENDER,REG_DATE,STATE,READ_STATE,UID,TIMEID
	   FROM MESSAGE
	   WHERE
	   (ID_SENDER=:USER_ID OR ID_RECEIVER=:USER_ID)
	   AND CONTENT_TYPE=:FILE_TYPE
	   ORDER BY ID DESC
	   INTO :ID_MESSAGE,:ID_SENDER,:REG_DATE,:STATE,:IS_READ,:UID,:TIMEID
	   DO
	   SUSPEND;
    
    END
    ELSE IF(:LAST_ID_MESSAGE<=0 AND :QUANTITY>0) THEN
    BEGIN
       FOR 
   	   SELECT FIRST(:QUANTITY) ID,ID_SENDER,REG_DATE,STATE,READ_STATE,UID,TIMEID
   	   FROM MESSAGE
   	   WHERE
   	   (ID_SENDER=:USER_ID OR ID_RECEIVER=:USER_ID)
   	   AND CONTENT_TYPE=:FILE_TYPE
   	   ORDER BY ID DESC
           INTO :ID_MESSAGE,:ID_SENDER,:REG_DATE,:STATE,:IS_READ,:UID,:TIMEID
   	   DO
   	   SUSPEND;
    END
    ELSE IF(:LAST_ID_MESSAGE>0 AND :QUANTITY<=0) THEN
    BEGIN
       FOR 
           SELECT ID,ID_SENDER,REG_DATE,STATE,READ_STATE,UID,TIMEID
   	   FROM MESSAGE
   	   WHERE
   	   (ID_SENDER=:USER_ID OR ID_RECEIVER=:USER_ID)
   	   AND CONTENT_TYPE=:FILE_TYPE
       AND ID<:LAST_ID_MESSAGE
   	   ORDER BY ID DESC
   	   INTO :ID_MESSAGE,:ID_SENDER,:REG_DATE,:STATE,:IS_READ,:UID,:TIMEID
   	   DO
   	   SUSPEND;
    END
     ELSE IF(:LAST_ID_MESSAGE>0 AND :QUANTITY>0) THEN
    BEGIN
       FOR 
   	   SELECT FIRST(:QUANTITY) ID,ID_SENDER,REG_DATE,STATE,READ_STATE,UID,TIMEID
   	   FROM MESSAGE
   	   WHERE
   	   (ID_SENDER=:USER_ID OR ID_RECEIVER=:USER_ID)
   	   AND CONTENT_TYPE=:FILE_TYPE
           AND ID<:LAST_ID_MESSAGE
   	   ORDER BY ID DESC
           INTO :ID_MESSAGE,:ID_SENDER,:REG_DATE,:STATE,:IS_READ,:UID,:TIMEID
   	   DO
   	   SUSPEND;
    END
END