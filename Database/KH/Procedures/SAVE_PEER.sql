/* Definition for the SAVE_PEER procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save profile of a peer.
--
-- Parameters:
-- USERNAME - The user name of peer.
-- TOKEN - The token of peer.
-- CERTIFICATE - The X509 certiticate of peer.
-- ADDRESS - The address of service published by peer.
-- SRVTYPE - The type of service published by peer.
-- HEX_COLOR - Hexadecimal color of peer profile.
-- INITIALS - The initials of peer user name.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_PEER
(
  USERNAME TYPE OF COLUMN PEER.USER_NAME,
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  CERTIFICATE TYPE OF COLUMN PEER.CERT,
  ADDRESS TYPE OF COLUMN PEER_ADDRESS.ADDRESS,
  SRVTYPE TYPE OF COLUMN PEER_ADDRESS.SERV_TYPE,
  HEX_COLOR TYPE OF COLUMN PEER.HEX_COLOR default null,
  INITIALS TYPE OF COLUMN PEER.INITIALS default null
)
AS

DECLARE VARIABLE ID_TOKEN INT;
DECLARE VARIABLE TODAY TIMESTAMP;

BEGIN
    TODAY=(SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE);    
    
    SELECT ID FROM PEER 
    WHERE
    TOKEN=:TOKEN
   
    INTO :ID_TOKEN;
    
    IF(ROW_COUNT=0) THEN
    BEGIN
       INSERT INTO PEER(USER_NAME,TOKEN,REG_DATE,CERT,HEX_COLOR,INITIALS)
       VALUES(:USERNAME,:TOKEN,:TODAY,:CERTIFICATE,:HEX_COLOR,:INITIALS)
       RETURNING ID
       INTO :ID_TOKEN;
    END
    ELSE IF(:HEX_COLOR IS NOT NULL AND :INITIALS IS NOT NULL) THEN
    BEGIN
       UPDATE PEER
       SET HEX_COLOR=:HEX_COLOR
       WHERE
       HEX_COLOR IS NULL
       AND TOKEN=:TOKEN;
       
       UPDATE PEER
       SET INITIALS=:INITIALS
       WHERE
       INITIALS IS NULL
       AND TOKEN=:TOKEN;
    END
    
    UPDATE OR INSERT INTO PEER_ADDRESS(ID_TOKEN,SERV_TYPE,ADDRESS,REG_DATE,ENABLED)
    VALUES(:ID_TOKEN,:SRVTYPE,:ADDRESS,:TODAY,'TRUE')
    MATCHING(ID_TOKEN,SERV_TYPE);
    
END