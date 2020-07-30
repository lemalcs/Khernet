/* Definition for the GET_PEER_ADDRESS procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-03-22
-- Autor: Luis Lema
--
-- Description: 
-- Get the address of a service that peer publish.
--
-- Parameters:
-- TOKEN - The token of peer.
-- SRVTYPE - The type of service to get to.
--           Communicator service = "COMMSRV"
--           File service = "FILESRV"
--           Notifier service = "NOTIFSRV"
--
-- Returns:
-- The address of service.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PEER_ADDRESS
(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  SRVTYPE TYPE OF COLUMN PEER_ADDRESS.SERV_TYPE
)
RETURNS
(
  ADDRESS TYPE OF COLUMN PEER_ADDRESS.ADDRESS
)
AS
BEGIN

   FOR SELECT ADDRESS FROM PEER A JOIN PEER_ADDRESS B
   ON A.ID=B.ID_TOKEN
   WHERE
   B.SERV_TYPE=:SRVTYPE
   AND A.TOKEN=:TOKEN
   INTO :ADDRESS
   DO
   SUSPEND;

END