/* Definition for the GET_SERVICE_ADDRESSES procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-11-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of address of a type of service.
--
-- Parameters:
-- SERVICE_TYPE - The type of service each peer publish.
--
-- Returns:
-- The list of service addresses.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_SERVICE_ADDRESSES
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
   AND A.SERV_TYPE=:SERVICE_TYPE
   INTO :TOKEN,:ADDRESS
   DO
   SUSPEND;
END