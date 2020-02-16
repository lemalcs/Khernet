/* Definition for the GET_PEERS procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the profiles of peers.
--
-- Returns:
-- The list of profiles of peers.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PEERS
RETURNS
(
  USER_NAME TYPE OF COLUMN PEER.USER_NAME,
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  FULL_NAME TYPE OF COLUMN PEER.FULL_NAME,
  DISPLAY_NAME TYPE OF COLUMN PEER.DISPLAY_NAME,
  HEX_COLOR TYPE OF COLUMN PEER.HEX_COLOR,
  INITIALS TYPE OF COLUMN PEER.INITIALS
)
AS
BEGIN
   FOR SELECT USER_NAME,TOKEN,FULL_NAME,DISPLAY_NAME,HEX_COLOR,INITIALS
   FROM PEER
   INTO
   :USER_NAME,:TOKEN,:FULL_NAME,:DISPLAY_NAME,:HEX_COLOR,:INITIALS
   DO
   SUSPEND;
END