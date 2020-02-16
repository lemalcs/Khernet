/* Definition for the MARK_AS_READ_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Mark a single message as read.
--
-- Parameters:
-- ID_MESSAGE - The id of message.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE MARK_AS_READ_MESSAGE
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
AS

DECLARE VARIABLE ID_SENDER INTEGER;

BEGIN
    ID_SENDER=(SELECT ID FROM MESSAGE WHERE ID=:ID_MESSAGE);
    
    UPDATE MESSAGE
    SET READ_STATE='TRUE'
    WHERE
    ID=:ID_MESSAGE
    AND READ_STATE='FALSE';
    
    DELETE FROM NOTIFICATION
    WHERE
    ID_TOKEN=:ID_SENDER
    AND PROCESSED='FALSE'
    AND TYPE_NOT=0;
    
END