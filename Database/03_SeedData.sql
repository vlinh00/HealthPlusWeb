USE HealthPlus;
GO


/* =========================================================
   CATEGORIES
   ========================================================= */

INSERT INTO Categories
(
    Name,
    Description
)
VALUES
(
    N'Vitamin & Khoáng chất',
    N'Các sản phẩm bổ sung vitamin và khoáng chất'
),
(
    N'Thực phẩm bổ sung',
    N'Các sản phẩm bổ sung dinh dưỡng'
),
(
    N'Hỗ trợ tiêu hóa',
    N'Sản phẩm hỗ trợ hệ tiêu hóa'
),
(
    N'Hỗ trợ xương khớp',
    N'Sản phẩm hỗ trợ xương và khớp'
),
(
    N'Hỗ trợ tim mạch',
    N'Sản phẩm hỗ trợ sức khỏe tim mạch'
);
GO


/* =========================================================
   PRODUCTS
   ========================================================= */

INSERT INTO Products
(
    CategoryId,
    Name,
    Description,
    Price,
    Stock,
    ImageUrl
)
VALUES
(
    1,
    N'Vitamin C 1000mg',
    N'Viên uống bổ sung vitamin C',
    250000,
    100,
    N'/images/vitamin-c.jpg'
),
(
    1,
    N'Vitamin D3',
    N'Sản phẩm bổ sung vitamin D3',
    180000,
    80,
    N'/images/vitamin-d3.jpg'
),
(
    2,
    N'Omega 3 Fish Oil',
    N'Dầu cá Omega 3',
    320000,
    60,
    N'/images/omega3.jpg'
),
(
    3,
    N'Probiotic',
    N'Sản phẩm bổ sung lợi khuẩn',
    290000,
    50,
    N'/images/probiotic.jpg'
),
(
    4,
    N'Calcium Plus',
    N'Sản phẩm bổ sung canxi',
    350000,
    70,
    N'/images/calcium.jpg'
),
(
    5,
    N'CoQ10',
    N'Sản phẩm hỗ trợ sức khỏe tim mạch',
    420000,
    40,
    N'/images/coq10.jpg'
);
GO