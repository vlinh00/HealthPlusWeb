```sql
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
   PRODUCTS - 30 PRODUCTS
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

/* =========================================================
   CATEGORY 1: VITAMIN & KHOÁNG CHẤT
   ========================================================= */

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
    1,
    N'Vitamin E 400 IU',
    N'Sản phẩm bổ sung vitamin E',
    220000,
    65,
    N'/images/vitamin-e.jpg'
),
(
    1,
    N'Multivitamin Daily',
    N'Viên uống đa vitamin dành cho nhu cầu dinh dưỡng hằng ngày',
    350000,
    90,
    N'/images/multivitamin.jpg'
),
(
    1,
    N'Zinc 15mg',
    N'Sản phẩm bổ sung kẽm',
    160000,
    75,
    N'/images/zinc.jpg'
),
(
    1,
    N'Magnesium B6',
    N'Sản phẩm bổ sung magie và vitamin B6',
    240000,
    55,
    N'/images/magnesium-b6.jpg'
),

/* =========================================================
   CATEGORY 2: THỰC PHẨM BỔ SUNG
   ========================================================= */

(
    2,
    N'Omega 3 Fish Oil',
    N'Dầu cá Omega 3',
    320000,
    60,
    N'/images/omega3.jpg'
),
(
    2,
    N'Whey Protein',
    N'Bột protein bổ sung dinh dưỡng cho người vận động',
    890000,
    35,
    N'/images/whey-protein.jpg'
),
(
    2,
    N'Collagen Peptides',
    N'Bột collagen peptide bổ sung vào chế độ dinh dưỡng',
    520000,
    45,
    N'/images/collagen.jpg'
),
(
    2,
    N'Vitamin B Complex',
    N'Sản phẩm bổ sung nhóm vitamin B',
    270000,
    70,
    N'/images/vitamin-b-complex.jpg'
),
(
    2,
    N'Plant Protein',
    N'Bột protein có nguồn gốc thực vật',
    650000,
    30,
    N'/images/plant-protein.jpg'
),
(
    2,
    N'Electrolyte Powder',
    N'Bột pha uống bổ sung chất điện giải',
    190000,
    50,
    N'/images/electrolyte.jpg'
),

/* =========================================================
   CATEGORY 3: HỖ TRỢ TIÊU HÓA
   ========================================================= */

(
    3,
    N'Probiotic',
    N'Sản phẩm bổ sung lợi khuẩn',
    290000,
    50,
    N'/images/probiotic.jpg'
),
(
    3,
    N'Digestive Enzymes',
    N'Sản phẩm bổ sung enzyme tiêu hóa',
    310000,
    40,
    N'/images/digestive-enzymes.jpg'
),
(
    3,
    N'Fiber Plus',
    N'Sản phẩm bổ sung chất xơ',
    230000,
    65,
    N'/images/fiber-plus.jpg'
),
(
    3,
    N'Prebiotic Fiber',
    N'Sản phẩm bổ sung chất xơ prebiotic',
    280000,
    45,
    N'/images/prebiotic.jpg'
),
(
    3,
    N'Probiotic Daily 10 Billion',
    N'Sản phẩm bổ sung lợi khuẩn với hàm lượng công bố 10 tỷ CFU',
    390000,
    38,
    N'/images/probiotic-daily.jpg'
),
(
    3,
    N'Ginger Digest',
    N'Sản phẩm bổ sung chiết xuất gừng',
    210000,
    42,
    N'/images/ginger-digest.jpg'
),

/* =========================================================
   CATEGORY 4: HỖ TRỢ XƯƠNG KHỚP
   ========================================================= */

(
    4,
    N'Calcium Plus',
    N'Sản phẩm bổ sung canxi',
    350000,
    70,
    N'/images/calcium-plus.jpg'
),
(
    4,
    N'Glucosamine 1500mg',
    N'Sản phẩm bổ sung glucosamine',
    480000,
    40,
    N'/images/glucosamine.jpg'
),
(
    4,
    N'Collagen Type II',
    N'Sản phẩm bổ sung collagen type II',
    560000,
    32,
    N'/images/collagen-type-ii.jpg'
),
(
    4,
    N'Calcium D3 K2',
    N'Sản phẩm bổ sung canxi và vitamin D3, K2',
    420000,
    55,
    N'/images/calcium-d3-k2.jpg'
),
(
    4,
    N'Joint Support',
    N'Sản phẩm bổ sung dưỡng chất dành cho người quan tâm đến xương khớp',
    590000,
    28,
    N'/images/joint-support.jpg'
),
(
    4,
    N'Magnesium Calcium Zinc',
    N'Sản phẩm bổ sung magie, canxi và kẽm',
    330000,
    48,
    N'/images/magnesium-calcium-zinc.jpg'
),

/* =========================================================
   CATEGORY 5: HỖ TRỢ TIM MẠCH
   ========================================================= */

(
    5,
    N'CoQ10',
    N'Sản phẩm bổ sung CoQ10',
    420000,
    40,
    N'/images/coq10.jpg'
),
(
    5,
    N'Fish Oil 1000mg',
    N'Dầu cá bổ sung omega-3',
    280000,
    60,
    N'/images/fish-oil.jpg'
),
(
    5,
    N'Plant Sterols',
    N'Sản phẩm bổ sung sterol thực vật',
    450000,
    25,
    N'/images/plant-sterols.jpg'
),
(
    5,
    N'Garlic Extract',
    N'Sản phẩm bổ sung chiết xuất tỏi',
    200000,
    50,
    N'/images/garlic-extract.jpg'
),
(
    5,
    N'Flaxseed Oil',
    N'Dầu hạt lanh bổ sung chất béo có nguồn gốc thực vật',
    260000,
    45,
    N'/images/flaxseed-oil.jpg'
),
(
    5,
    N'Potassium Supplement',
    N'Sản phẩm bổ sung kali',
    240000,
    30,
    N'/images/potassium.jpg'
);
GO

/* =========================================================
   VERIFY SEED DATA
   ========================================================= */

SELECT
    c.Name AS CategoryName,
    COUNT(p.Id) AS ProductCount
FROM Categories c
LEFT JOIN Products p ON p.CategoryId = c.Id
GROUP BY c.Id, c.Name
ORDER BY c.Id;

SELECT COUNT(*) AS TotalProducts
FROM Products;
GO
```