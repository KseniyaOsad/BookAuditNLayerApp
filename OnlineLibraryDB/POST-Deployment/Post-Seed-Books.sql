SET IDENTITY_INSERT Books ON 
INSERT INTO Books (Id, Name, Description, Genre) 
VALUES (1, 'c# 7.0 in a nutshell', 'the definitive reference', 8), (2, 'Harry Potter', ' The boy who lived', 3);
SET IDENTITY_INSERT Books OFF ;