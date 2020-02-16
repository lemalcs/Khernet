
------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Create database file for Firebird Engine. This database will be used to 
-- store configurations used by application.
-- The name of this database depends of the name of application executable
-- file plus .dat extension.
------------------------------------------------------------------------------

CREATE DATABASE 'DATABASE_FILE_PATH\CONFIG.dat'
  USER 'SYSDBA'
  PAGE_SIZE = 4096
  DEFAULT CHARACTER SET NONE;