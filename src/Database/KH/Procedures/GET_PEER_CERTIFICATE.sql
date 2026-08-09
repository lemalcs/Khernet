/* Definition for the GET_PEER_CERTIFICATE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the vertificate of a peer.
--
-- Parameters:
-- TOKEN - The token of peer.
--
-- Returns:
-- A byte array containing the certificate file.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PEER_CERTIFICATE
(
  TOKEN TYPE OF COLUMN PEER.TOKEN
)
RETURNS
(
  CERTIFICATE TYPE OF COLUMN PEER.CERT
)
AS
BEGIN

   FOR SELECT CERT FROM PEER
   WHERE
   TOKEN=:TOKEN
   INTO :CERTIFICATE
   DO
   SUSPEND;

END