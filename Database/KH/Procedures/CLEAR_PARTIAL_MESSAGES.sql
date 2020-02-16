/* Definition for the CLEAR_PARTIAL_MESSAGES procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Deletes all messages fragments used when sending text messages.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE CLEAR_PARTIAL_MESSAGES
AS
BEGIN
   DELETE FROM PARTIAL_MESSAGE;
END