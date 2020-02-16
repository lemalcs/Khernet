/* Definition for the GET_PEERS_ADDRESS procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get service addresses of peers.
--
-- Parameters:
-- SRVTYPE - The type of service
--
-- Returns:
-- The list of addresses where services are published.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PEERS_ADDRESS
(
  SRVTYPE TYPE OF COLUMN PEER_ADDRESS.SERV_TYPE
)
RETURNS
(
  ADDRESS TYPE OF COLUMN PEER_ADDRESS.ADDRESS
)
AS
BEGIN

   FOR SELECT ADDRESS FROM PEER_ADDRESS
   WHERE
   SERV_TYPE=:SRVTYPE
   INTO :ADDRESS
   DO
   SUSPEND;

END