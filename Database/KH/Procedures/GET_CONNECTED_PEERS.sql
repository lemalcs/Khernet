/* Definition for the GET_CONNECTED_PEERS procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of peers that are online.
--
-- Parameters:
-- SERVICE_TYPE - The type of service each peer publish
--
-- Returns:
-- The list of service addresses.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_CONNECTED_PEERS
(
  SERVICE_TYPE TYPE OF COLUMN PEER_ADDRESS.SERV_TYPE
)
RETURNS
(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  ADDRESS TYPE OF COLUMN PEER_ADDRESS.ADDRESS
)
AS
BEGIN
   FOR SELECT B.TOKEN,A.ADDRESS FROM PEER_ADDRESS A JOIN PEER B
   ON A.ID_TOKEN=B.ID
   WHERE
   B.STATE<>0 --Filter offline state
   AND A.SERV_TYPE=:SERVICE_TYPE
   INTO :TOKEN,:ADDRESS
   DO
   SUSPEND;
END