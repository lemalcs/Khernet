/* Definition for the BI_MESSAGE_ID trigger :  */

CREATE TRIGGER BI_MESSAGE_ID FOR MESSAGE
ACTIVE BEFORE 
  INSERT
POSITION 0
AS
begin
   if(NEW.ID is null)then
       NEW.ID=GEN_ID(MESSAGE_ID_GEN,1);
end