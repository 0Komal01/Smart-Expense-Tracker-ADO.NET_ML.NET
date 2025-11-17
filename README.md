# 💰 Expense Tracker with ML Predictions ( ADO.NET+ ASP.NET Web Forms+ ML.NET)

## 🧠 Overview
This is a full-featured Expense Tracking System built using **ASP.NET Web Forms (C#)** with **ADO.NET** for database operations and **ML.NET** for machine learning predictions.

The system allows users to manage income, expenses, and budgets — while using AI to forecast spending, detect anomalies, cluster spending patterns, and predict overspending risks.

## ⚙️ Tech Stack
- **Frontend:** ASP.NET Web Forms, HTML, CSS, JavaScript  
- **Backend:** C#, ADO.NET  
- **Database:** SQL Server (LocalDB)  
- **Machine Learning:** ML.NET (Forecasting, Clustering, Classification)

## 🚀 Features
1. **User Authentication (Login/Register)**
2. **CRUD Operations** for Categories, Income, and Budget  
3. **Expense Forecasting (Time Series Prediction)**
4. **Anomaly Detection (Z-Score Based)**
5. **Spending Pattern Clustering (K-Means)**
6. **Overspending Risk Prediction (Binary Classification)**
7. **Dynamic Dashboard with Smart Tips**
8. **Export to PDF (iTextSharp)**
   

## 📊 ML Models Used
| 🧠 Model Name                 | 🧩 Type / Algorithm                  | 🎯 Purpose Description                                          |
| ----------------------------- | ------------------------------------ | --------------------------------------------------------------- |
| **Time Series Forecasting**   | Exponential Smoothing (Holt-Winters) | Predicts **future expenses** based on past spending trends.     |
| **Z-Score Anomaly Detection** | Statistical Analysis                 | Detects **unusual spending spikes or drops** in daily expenses. |
| **K-Means Clustering**        | Unsupervised Learning                | Groups **spending categories** with similar behavior patterns.  |
| **Averaged Perceptron**       | Binary Classification (ML.NET)       | Predicts the **risk of overspending** for the current month.    |



## 🗄️ Database Design
Includes tables:
- **Users**
- **Categories**
- **Income**
- **Budget**

Each table is linked by `UserID` for personalized data tracking.


## 🧩 Why ADO.NET?
ADO.NET gives direct database control with clear SQL queries, which makes CRUD operations simpler to implement and debug for a small-scale project.  
MVC is better for large enterprise apps — but ADO.NET is ideal for learning and direct data handling.

## 🧠 Machine Learning Integration
All ML models are trained dynamically using **ML.NET** each time the page loads or data changes:
- Clustering and classification models are trained in real time.
- Predictions update automatically for each logged-in user.



---

## 📸 Implementation Snapshots
<img width="600" height="272" alt="image" src="https://github.com/user-attachments/assets/90e06797-33fd-4be4-bb5b-648a2642bbf3" />
<img width="600" height="272" alt="image" src="https://github.com/user-attachments/assets/36c94e49-12e7-4faa-b908-17c7da634620" />
<img width="600" height="272" alt="image" src="https://github.com/user-attachments/assets/acbd4d7a-8219-4f4a-85b5-2ed1b82404b5" />
<img width="600" height="272" alt="image" src="https://github.com/user-attachments/assets/83478a18-d7d9-4cb6-9b12-8f0f787c82c7" />
<img width="600" height="272" alt="image" src="https://github.com/user-attachments/assets/16761e04-ebc0-433a-91ce-d9784d0ee767" />

[Project Report.docx](https://github.com/user-attachments/files/23394807/Project.Report.docx)


## 👩‍💻 Author
**Komal Kumari**  
📧 [Email : komal123ydv456@gmail.com & LinkedIn : https://www.linkedin.com/in/komal-kumari-a10476289/]  
🧩 Btech Computer Engineering

