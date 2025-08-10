insert into Persons
(FirstName, LastName, ImageURL, Phone, Email, Password, CountryId, IsActive, UserTypeId)
values
(
    'Waleed',
    'Sameer',
    'https://res.cloudinary.com/dlu3aolnh/image/upload/v1750941990/user-square_rhmoxo.png',
    '0796102413',
    'waleed@gmail.com',
    '1hqA8azv3A/XlSi5V0x7QA==',
    1,
    1,
    2
),
(
    'Omar',
    'Khaled',
    'https://res.cloudinary.com/dlu3aolnh/image/upload/v1750941990/user-square_rhmoxo.png',
    '0798765432',
    'omar.khaled@gmail.com',
    '1hqA8azv3A/XlSi5V0x7QA==',
    1,
    1,
    1
);


insert into Recruiters (PersonId,IsInstructor,UniversityId)
values 
(1,1,1)

insert into Seekers(PersonId,Job_TypeId,Job_LevelId,IsStudent,UniversityId,UniversityStudentNumber,StudyingYear)
values
(2,22,1,1,1,'2332150',3)
