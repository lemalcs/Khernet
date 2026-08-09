/* Definition for the GET_PEER_AVATAR procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get avatar of a peer.
--
-- Parameters:
-- TOKEN - The token of a peer.
--
-- Returns:
-- An array of bytes containing the avatar (image file).
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PEER_AVATAR
(
  TOKEN TYPE OF COLUMN PEER.TOKEN
)
RETURNS
(
  AVATAR TYPE OF COLUMN PEER.AVATAR
)
AS
BEGIN
   FOR SELECT AVATAR FROM PEER
   WHERE
   TOKEN=:TOKEN
   INTO :AVATAR
   DO
   SUSPEND;
END