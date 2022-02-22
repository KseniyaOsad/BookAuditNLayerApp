IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo'))  
BEGIN
    
    IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE  TABLE_NAME = 'Books'))  
    BEGIN
           DELETE FROM [dbo].[Books] WHERE Id IN (1, 2);
    END

    IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE  TABLE_NAME = 'Authors'))  
    BEGIN
            DELETE FROM [dbo].[Authors] WHERE Id IN (1, 2, 3);
    END

    IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE  TABLE_NAME = 'Tags'))  
    BEGIN
            DELETE FROM [dbo].[Tags] WHERE Id IN (1, 2, 3);
    END

    IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE  TABLE_NAME = 'Users'))  
    BEGIN
            DELETE FROM [dbo].[Users] WHERE Id IN (1, 2);
    END

    
END