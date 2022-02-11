INSERT INTO Tags (Name) VALUES ('Programming'), ('With good end'), ('For family');

INSERT INTO Authors (Name) VALUES ('Ben Albahari'), ('Joseph Albahari'), ('J. K. Rowling');

INSERT INTO Books (Name, Description, Genre) VALUES ('c# 7.0 in a nutshell', 'the definitive reference', 8), ('Harry Potter', ' The boy who lived', 3);

INSERT INTO BookTag (BooksId, TagsId) VALUES (1, 1), (2, 2), (2, 3);

INSERT INTO AuthorBook (BooksId, AuthorsId) VALUES (1, 1), (1, 2), (2, 3);

INSERT INTO Users (Name, Email, DateOfBirth) VALUES ('Viktor', 'victor@gmail.com', '1999-01-01'), ('Anya', 'anya@gmail.com', '1989-11-10');