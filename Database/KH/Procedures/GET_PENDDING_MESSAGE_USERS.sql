/* Definition for the GET_PENDDING_MESSAGE_USERS procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of peers for whom there are pendding message to sent.
--
-- Parameters:
-- IDKEY - The name of configuration, this value is unique across others.
--
-- Returns:
-- The list of tokens of peers.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PENDDING_MESSAGE_USERS
RETURNS
(
  RECEIPT_TOKEN TYPE OF COLUMN PEER.TOKEN
)
AS
DECLARE VARIABLE SENDERTOKEN TYPE OF COLUMN PEER.TOKEN;
BEGIN
   SENDERTOKEN=(SELECT TOKEN FROM ACCOUNT WHERE ID=1);

   FOR SELECT DISTINCT B.TOKEN
   FROM MESSAGE A JOIN PEER B
   ON A.ID_RECEIPT=B.ID
   JOIN PENDDING_MESSAGE C
   ON A.ID=C.ID_MESSAGE
   AND A.ID_RECEIPT=C.ID_RECEIPT
   ORDER BY B.STATE DESC
   INTO :RECEIPT_TOKEN
   DO
   SUSPEND;
END