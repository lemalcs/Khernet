/* Structure for the ACCOUNT table :  */

CREATE TABLE ACCOUNT 
(
  ID INTEGER NOT NULL,
  TOKEN VARCHAR(128) CHARACTER SET UTF8 NOT NULL,
  STATE SMALLINT,
  USER_NAME VARCHAR(64) CHARACTER SET UTF8,
  SLOGAN VARCHAR(256) CHARACTER SET UTF8,
  AVATAR BLOB,
  GROUP_NAME VARCHAR(64) CHARACTER SET UTF8,
  REG_DATE TIMESTAMP NOT NULL,
  FULL_NAME BLOB
);

ALTER TABLE ACCOUNT ADD PRIMARY KEY (ID,TOKEN);