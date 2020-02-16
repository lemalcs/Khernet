/* Definition for the GET_PEER_SERV_ADDRESS procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get addres of service published by a peer.
--
-- Parameters:
-- TOKEN - The token of peer.
-- SERV_TYPE - The type of service.
--
-- Returns:
-- The address of service.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PEER_SERV_ADDRESS
(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  SERV_TYPE TYPE OF COLUMN PEER_ADDRESS.SERV_TYPE
)
RETURNS
(
  SERV_ADDRESS TYPE OF COLUMN PEER_ADDRESS.ADDRESS
)
AS
BEGIN
   FOR SELECT
   B.ADDRESS
   FROM PEER A JOIN PEER_ADDRESS B
   ON A.ID=B.ID_TOKEN
   WHERE
   A.TOKEN=:TOKEN
   AND B.SERV_TYPE=:SERV_TYPE
   INTO
   :SERV_ADDRESS
   DO
   SUSPEND;
END