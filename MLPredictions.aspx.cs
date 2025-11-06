using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;

namespace Expense_Tracker
{
    public partial class MLPredictions : System.Web.UI.Page
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Komal\OneDrive\Important Folder\ExpenseTracker.mdf;Integrated Security=True;Connect Timeout=30";
        private MLContext mlContext;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            mlContext = new MLContext(seed: 0);

            if (!IsPostBack)
            {
                LoadUserProfile();
                LoadMLPredictions();
            }
        }

        private void LoadUserProfile()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Username, Email FROM Users WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string username = reader["Username"].ToString();
                    lblUsername.Text = username;
                    lblEmail.Text = reader["Email"].ToString();
                    lblUserInitial.Text = username.Substring(0, 1).ToUpper();
                }
                reader.Close();
            }
        }

        #region ML Model 1: Time Series Forecasting (Exponential Smoothing)
        private void LoadMLPredictions()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            // Get historical spending data
            List<ExpenseData> historicalData = GetHistoricalExpenses(userId);

            if (historicalData.Count < 10)
            {
                lblMLStatus.Text = "⚠️ Need at least 10 days of expenses to predict";
                pnlMLPrediction.Visible = false;
                return;
            }

            try
            {
                // Time Series Forecasting
                var forecastResult = ForecastExpenses(historicalData);

                lblNextDayPrediction.Text = $"₹{forecastResult.ForecastedExpenses[0]:N2}";
                lblNext7DayPrediction.Text = $"₹{forecastResult.ForecastedExpenses.Take(7).Sum():N2}";
                lblNext30DayPrediction.Text = $"₹{forecastResult.ForecastedExpenses.Sum():N2}";

                // Calculate confidence based on prediction intervals
                int confidence = CalculateMLConfidence(forecastResult);
                lblMLConfidence.Text = $"{confidence}%";
                lblTrainingDataPoints.Text = historicalData.Count.ToString();

                // Generate smart tips based on forecasts
                string smartTip = GenerateSmartTip(forecastResult, historicalData);
                lblSmartTip.Text = smartTip;

                // Anomaly Detection
                DetectSpendingAnomalies(userId, historicalData);

                // Monthly Spending Trend
                LoadMonthlySpendingTrend(userId);

                // Clustering Analysis
                PerformCategoryClustering(userId);

                // Binary Classification - Overspend Risk
                PredictOverspendRisk(userId);

                lblMLStatus.Text = "✅ All predictions ready! Your AI analysis is complete.";
                pnlMLPrediction.Visible = true;
            }
            catch (Exception ex)
            {
                lblMLStatus.Text = $"❌ ML Error: {ex.Message}";
                pnlMLPrediction.Visible = false;
            }
        }

        private List<ExpenseData> GetHistoricalExpenses(int userId)
        {
            List<ExpenseData> data = new List<ExpenseData>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        CAST(CreatedDate AS DATE) as ExpenseDate,
                        SUM(Amount) as DailyAmount,
                        COUNT(*) as TransactionCount
                    FROM Categories
                    WHERE UserID = @UserID
                        AND CreatedDate >= DATEADD(DAY, -90, GETDATE())
                    GROUP BY CAST(CreatedDate AS DATE)
                    ORDER BY ExpenseDate";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    data.Add(new ExpenseData
                    {
                        Date = Convert.ToDateTime(reader["ExpenseDate"]),
                        Amount = Convert.ToSingle(reader["DailyAmount"]),
                        TransactionCount = Convert.ToInt32(reader["TransactionCount"])
                    });
                }
                reader.Close();
            }

            return data;
        }

        private ForecastResult ForecastExpenses(List<ExpenseData> historicalData)
        {
            try
            {
                // Triple Exponential Smoothing (Holt-Winters) for Time Series Forecasting
                int horizon = 30;
                float[] forecasts = new float[horizon];
                float[] lowerBounds = new float[horizon];
                float[] upperBounds = new float[horizon];

                // Sort data by date
                var sortedData = historicalData.OrderBy(d => d.Date).ToList();

                // Calculate statistics
                float mean = sortedData.Average(d => d.Amount);
                double variance = sortedData.Average(d => Math.Pow(d.Amount - mean, 2));
                float stdDev = (float)Math.Sqrt(variance);

                // Exponential smoothing parameters
                float alpha = 0.3f;  // Level smoothing
                float beta = 0.1f;   // Trend smoothing
                float gamma = 0.2f;  // Seasonal smoothing

                // Initialize level and trend
                float level = sortedData.Take(7).Average(d => d.Amount);
                float trend = 0f;

                // Calculate trend from first week
                if (sortedData.Count >= 14)
                {
                    float firstWeek = sortedData.Take(7).Average(d => d.Amount);
                    float secondWeek = sortedData.Skip(7).Take(7).Average(d => d.Amount);
                    trend = (secondWeek - firstWeek) / 7;
                }

                // Calculate seasonal indices (7-day cycle)
                float[] seasonalIndices = new float[7];
                for (int i = 0; i < 7; i++)
                {
                    var dayValues = sortedData.Where((d, idx) => idx % 7 == i).Select(d => d.Amount).ToList();
                    if (dayValues.Count > 0)
                    {
                        seasonalIndices[i] = dayValues.Average() / mean;
                    }
                    else
                    {
                        seasonalIndices[i] = 1.0f;
                    }
                }

                // Apply exponential smoothing
                float lastLevel = level;
                float lastTrend = trend;

                for (int i = sortedData.Count - 7; i < sortedData.Count; i++)
                {
                    if (i >= 0)
                    {
                        int seasonIdx = i % 7;
                        float value = sortedData[i].Amount;

                        float newLevel = alpha * (value / seasonalIndices[seasonIdx]) + (1 - alpha) * (lastLevel + lastTrend);
                        float newTrend = beta * (newLevel - lastLevel) + (1 - beta) * lastTrend;

                        lastLevel = newLevel;
                        lastTrend = newTrend;
                    }
                }

                // Generate forecasts
                for (int i = 0; i < horizon; i++)
                {
                    int seasonIdx = (sortedData.Count + i) % 7;
                    float forecast = (lastLevel + (i + 1) * lastTrend) * seasonalIndices[seasonIdx];

                    forecasts[i] = Math.Max(0, forecast);

                    // Confidence intervals widen over time
                    float confidenceWidth = stdDev * 1.96f * (1 + i * 0.05f);
                    lowerBounds[i] = Math.Max(0, forecasts[i] - confidenceWidth);
                    upperBounds[i] = forecasts[i] + confidenceWidth;
                }

                return new ForecastResult
                {
                    ForecastedExpenses = forecasts,
                    LowerBounds = lowerBounds,
                    UpperBounds = upperBounds
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Forecasting error: {ex.Message}");
            }
        }

        private int CalculateMLConfidence(ForecastResult result)
        {
            float avgPrediction = result.ForecastedExpenses.Average();
            if (avgPrediction == 0) return 50;

            float avgIntervalWidth = 0;
            for (int i = 0; i < result.ForecastedExpenses.Length; i++)
            {
                avgIntervalWidth += (result.UpperBounds[i] - result.LowerBounds[i]);
            }
            avgIntervalWidth /= result.ForecastedExpenses.Length;

            float confidenceRatio = 1 - (avgIntervalWidth / (avgPrediction * 2));
            int confidence = (int)(confidenceRatio * 100);

            return Math.Max(50, Math.Min(95, confidence));
        }

        private string GenerateSmartTip(ForecastResult result, List<ExpenseData> historicalData)
        {
            float nextDayForecast = result.ForecastedExpenses[0];
            float weekForecast = result.ForecastedExpenses.Take(7).Sum();
            float monthForecast = result.ForecastedExpenses.Sum();
            float avgDailySpending = historicalData.Average(d => d.Amount);
            float currentMonthSpending = GetCurrentMonthSpending();

            // Generate contextual tips
            if (monthForecast > 100000)
            {
                return "💰 High spending month ahead! Consider setting a daily budget of ₹" + (monthForecast / 30).ToString("N0") + " to stay on track.";
            }
            else if (nextDayForecast > avgDailySpending * 1.5)
            {
                return "📊 Tomorrow's forecast is 50% higher than usual. Review planned expenses and prioritize essentials.";
            }
            else if (weekForecast > avgDailySpending * 10)
            {
                return "🎯 Next week shows elevated spending. Try the 50/30/20 rule: 50% needs, 30% wants, 20% savings.";
            }
            else if (monthForecast < avgDailySpending * 20)
            {
                return "✨ Great job! Your spending is trending lower. Consider saving the extra ₹" + ((avgDailySpending * 30) - monthForecast).ToString("N0") + " this month.";
            }
            else if (currentMonthSpending > monthForecast * 0.7)
            {
                return "⚠️ You've already spent 70% of predicted monthly budget. Time to cut back on non-essentials.";
            }
            else
            {
                return "📈 Spending looks normal. Stay consistent and track daily expenses to maintain this pattern.";
            }
        }

        private float GetCurrentMonthSpending()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT ISNULL(SUM(Amount), 0) as Total
                    FROM Categories
                    WHERE UserID = @UserID
                        AND MONTH(CreatedDate) = MONTH(GETDATE())
                        AND YEAR(CreatedDate) = YEAR(GETDATE())";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToSingle(result);
            }
        }
        #endregion

        #region ML Model 2: Anomaly Detection (Z-Score Method)
        private void DetectSpendingAnomalies(int userId, List<ExpenseData> historicalData)
        {
            try
            {
                if (historicalData.Count < 5)
                {
                    pnlAnomalies.Visible = false;
                    return;
                }

                // Calculate Z-Score for anomaly detection
                float mean = historicalData.Average(d => d.Amount);
                double variance = historicalData.Average(d => Math.Pow(d.Amount - mean, 2));
                float stdDev = (float)Math.Sqrt(variance);

                List<AnomalyData> anomalies = new List<AnomalyData>();

                foreach (var data in historicalData)
                {
                    float zScore = stdDev > 0 ? (data.Amount - mean) / stdDev : 0;

                    // Detect anomalies (Z-Score > 2 is unusual)
                    if (Math.Abs(zScore) > 2.0f)
                    {
                        anomalies.Add(new AnomalyData
                        {
                            Date = data.Date,
                            Amount = data.Amount,
                            Score = zScore,
                            Type = zScore > 0 ? "Unusual Spike" : "Unusual Drop"
                        });
                    }
                }

                if (anomalies.Count > 0)
                {
                    rptAnomalies.DataSource = anomalies.OrderByDescending(a => Math.Abs(a.Score)).Take(5);
                    rptAnomalies.DataBind();
                    pnlAnomalies.Visible = true;
                    lblAnomalyCount.Text = anomalies.Count.ToString();

                    // Generate anomaly tip
                    var latestAnomaly = anomalies.OrderByDescending(a => a.Date).First();
                    if (latestAnomaly.Score > 3)
                    {
                        lblAnomalyTip.Text = "🚨 Very unusual spending detected! Verify if this was an emergency expense or planned purchase.";
                    }
                    else if (anomalies.Count > 5)
                    {
                        lblAnomalyTip.Text = "⚠️ Multiple spikes detected. Create a contingency fund to handle unexpected expenses better.";
                    }
                    else
                    {
                        lblAnomalyTip.Text = "💡 Occasional spikes are normal. Just ensure they don't become a pattern.";
                    }
                }
                else
                {
                    pnlAnomalies.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMLStatus.Text += $" | Anomaly Detection Error: {ex.Message}";
                pnlAnomalies.Visible = false;
            }
        }
        #endregion

        #region ML Model: Monthly Spending Trend Analysis
        private void LoadMonthlySpendingTrend(int userId)
        {
            try
            {
                List<MonthlyTrendData> trendData = GetMonthlyTrendData(userId);
                if (trendData.Count < 2)
                {
                    pnlSpendingTrend.Visible = false;
                    return;
                }

                float currentMonth = trendData.First().Amount;
                float lastMonth = trendData.Skip(1).First().Amount;
                float sixMonthAvg = trendData.Average(t => t.Amount);
                float maxAmount = trendData.Max(t => t.Amount);

                foreach (var data in trendData)
                {
                    data.Percentage = maxAmount > 0 ? (data.Amount / maxAmount) * 100 : 0;
                }

                lblCurrentMonth.Text = currentMonth.ToString("N0");
                lblLastMonth.Text = lastMonth.ToString("N0");
                lblSixMonthAvg.Text = sixMonthAvg.ToString("N0");

                float changePercent = lastMonth > 0 ? ((currentMonth - lastMonth) / lastMonth) * 100 : 0;
                if (changePercent > 0)
                {
                    lblCurrentTrend.Text = $"<span class='trend-up'>↑ {changePercent:N1}%</span>";
                }
                else if (changePercent < 0)
                {
                    lblCurrentTrend.Text = $"<span class='trend-down'>↓ {Math.Abs(changePercent):N1}%</span>";
                }
                else
                {
                    lblCurrentTrend.Text = "→ 0%";
                }

                rptMonthlyTrend.DataSource = trendData;
                rptMonthlyTrend.DataBind();

                if (currentMonth > sixMonthAvg * 1.3)
                {
                    lblTrendTip.Text = $"📈 Spending is 30% above your average! Try to reduce by ₹{(currentMonth - sixMonthAvg):N0} to get back on track.";
                }
                else if (currentMonth < sixMonthAvg * 0.7)
                {
                    lblTrendTip.Text = $"🎉 Excellent control! You're spending 30% less than usual. Keep it up!";
                }
                else if (changePercent > 20)
                {
                    lblTrendTip.Text = $"⚠️ Spending jumped {changePercent:N0}% from last month. Review what changed in your budget.";
                }
                else if (changePercent < -20)
                {
                    lblTrendTip.Text = $"💚 Great job! You reduced spending by {Math.Abs(changePercent):N0}% compared to last month.";
                }
                else
                {
                    lblTrendTip.Text = "📊 Spending is consistent with your average. Stay on track!";
                }

                pnlSpendingTrend.Visible = true;
            }
            catch (Exception ex)
            {
                lblMLStatus.Text += $" | Trend Error: {ex.Message}";
                pnlSpendingTrend.Visible = false;
            }
        }

        private List<MonthlyTrendData> GetMonthlyTrendData(int userId)
        {
            List<MonthlyTrendData> data = new List<MonthlyTrendData>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 6
                        YEAR(CreatedDate) as Year,
                        MONTH(CreatedDate) as Month,
                        SUM(Amount) as TotalAmount
                    FROM Categories
                    WHERE UserID = @UserID
                        AND CreatedDate >= DATEADD(MONTH, -6, GETDATE())
                    GROUP BY YEAR(CreatedDate), MONTH(CreatedDate)
                    ORDER BY YEAR(CreatedDate) DESC, MONTH(CreatedDate) DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int year = Convert.ToInt32(reader["Year"]);
                    int month = Convert.ToInt32(reader["Month"]);

                    data.Add(new MonthlyTrendData
                    {
                        MonthName = new DateTime(year, month, 1).ToString("MMM yyyy"),
                        Amount = Convert.ToSingle(reader["TotalAmount"]),
                        Percentage = 0 // Will be calculated later
                    });
                }
                reader.Close();
            }

            return data;
        }
        #endregion

        #region ML Model 3: Clustering (K-Means for Spending Patterns)
        private void PerformCategoryClustering(int userId)
        {
            try
            {
                // Get category spending patterns
                List<CategoryFeatures> categoryData = GetCategoryFeatures(userId);

                if (categoryData.Count < 3)
                {
                    pnlClustering.Visible = false;
                    return;
                }

                var dataView = mlContext.Data.LoadFromEnumerable(categoryData);

                int numClusters = Math.Min(3, categoryData.Count);

                // Feature engineering
                var pipeline = mlContext.Transforms.Concatenate(
                    "Features",
                    nameof(CategoryFeatures.TotalSpent),
                    nameof(CategoryFeatures.TransactionCount),
                    nameof(CategoryFeatures.AverageAmount),
                    nameof(CategoryFeatures.Frequency))
                    .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(mlContext.Clustering.Trainers.KMeans(
                        featureColumnName: "Features",
                        numberOfClusters: numClusters));

                var model = pipeline.Fit(dataView);
                var predictions = model.Transform(dataView);

                var clusterPredictions = mlContext.Data.CreateEnumerable<ClusterPrediction>(
                    predictions, reuseRowObject: false).ToList();

                // Group categories by cluster
                List<ClusterGroup> clusters = new List<ClusterGroup>();
                for (uint i = 0; i < numClusters; i++)
                {
                    var categoriesInCluster = categoryData
                        .Where((c, idx) => idx < clusterPredictions.Count && clusterPredictions[idx].PredictedClusterId == i)
                        .ToList();

                    if (categoriesInCluster.Count > 0)
                    {
                        clusters.Add(new ClusterGroup
                        {
                            ClusterId = (int)i + 1,
                            ClusterName = GetClusterName((int)i, categoriesInCluster),
                            Categories = string.Join(", ", categoriesInCluster.Select(c => c.CategoryName)),
                            TotalSpending = categoriesInCluster.Sum(c => c.TotalSpent),
                            Insight = GetClusterInsight((int)i, categoriesInCluster)
                        });
                    }
                }

                if (clusters.Count > 0)
                {
                    rptClusters.DataSource = clusters;
                    rptClusters.DataBind();
                    pnlClustering.Visible = true;

                    // Generate clustering tip
                    var highestCluster = clusters.OrderByDescending(c => c.TotalSpending).First();
                    if (highestCluster.TotalSpending > clusters.Sum(c => c.TotalSpending) * 0.5)
                    {
                        lblClusteringTip.Text = $"💸 '{highestCluster.ClusterName}' dominates your spending. Consider setting a cap on these expenses.";
                    }
                    else if (clusters.Any(c => c.ClusterName.Contains("Small Frequent")))
                    {
                        lblClusteringTip.Text = "☕ Small frequent expenses add up! Try tracking daily coffee/snack spending - you might be surprised.";
                    }
                    else
                    {
                        lblClusteringTip.Text = "✅ Well-balanced spending across categories. Keep maintaining this diversity.";
                    }
                }
                else
                {
                    pnlClustering.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMLStatus.Text += $" | Clustering Error: {ex.Message}";
                pnlClustering.Visible = false;
            }
        }

        private List<CategoryFeatures> GetCategoryFeatures(int userId)
        {
            List<CategoryFeatures> features = new List<CategoryFeatures>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        CategoryName,
                        SUM(Amount) as TotalSpent,
                        COUNT(*) as TransactionCount,
                        AVG(Amount) as AverageAmount,
                        DATEDIFF(DAY, MIN(CreatedDate), MAX(CreatedDate)) + 1 as DaySpan
                    FROM Categories
                    WHERE UserID = @UserID
                        AND CreatedDate >= DATEADD(MONTH, -3, GETDATE())
                    GROUP BY CategoryName
                    HAVING COUNT(*) >= 2";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int daySpan = Math.Max(1, Convert.ToInt32(reader["DaySpan"]));
                    int transactionCount = Convert.ToInt32(reader["TransactionCount"]);

                    features.Add(new CategoryFeatures
                    {
                        CategoryName = reader["CategoryName"].ToString(),
                        TotalSpent = Convert.ToSingle(reader["TotalSpent"]),
                        TransactionCount = transactionCount,
                        AverageAmount = Convert.ToSingle(reader["AverageAmount"]),
                        Frequency = (float)transactionCount / daySpan
                    });
                }
                reader.Close();
            }

            return features;
        }

        private string GetClusterName(int clusterId, List<CategoryFeatures> categories)
        {
            float avgAmount = categories.Average(c => c.AverageAmount);
            float avgFreq = categories.Average(c => c.Frequency);

            if (avgAmount > 2000 && avgFreq < 0.5f)
                return "💎 High-Value Occasional";
            else if (avgAmount < 500 && avgFreq > 1.0f)
                return "🔄 Small Frequent";
            else
                return "📊 Regular Spending";
        }

        private string GetClusterInsight(int clusterId, List<CategoryFeatures> categories)
        {
            float totalSpent = categories.Sum(c => c.TotalSpent);
            string topCategory = categories.OrderByDescending(c => c.TotalSpent).First().CategoryName;

            return $"Dominated by {topCategory} with ₹{totalSpent:N2} total spending";
        }
        #endregion

        #region ML Model 4: Binary Classification (Overspend Risk Prediction)
        private void PredictOverspendRisk(int userId)
        {
            try
            {
                // Get monthly spending history
                List<MonthlyData> monthlyData = GetMonthlyData(userId);

                if (monthlyData.Count < 3)
                {
                    pnlRiskPrediction.Visible = false;
                    return;
                }

                var dataView = mlContext.Data.LoadFromEnumerable(monthlyData);

                // Split data for training
                var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

                // Build classification pipeline using AveragedPerceptron
                var pipeline = mlContext.Transforms.Concatenate(
                    "Features",
                    nameof(MonthlyData.Income),
                    nameof(MonthlyData.Expenses),
                    nameof(MonthlyData.CategoryCount),
                    nameof(MonthlyData.TransactionCount))
                    .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(mlContext.BinaryClassification.Trainers.AveragedPerceptron(
                        labelColumnName: nameof(MonthlyData.IsOverspending),
                        featureColumnName: "Features",
                        numberOfIterations: 10));

                var model = pipeline.Fit(split.TrainSet);

                // Predict for current month
                var currentMonthData = GetCurrentMonthData(userId);
                var predictionEngine = mlContext.Model.CreatePredictionEngine<MonthlyData, OverspendPrediction>(model);
                var prediction = predictionEngine.Predict(currentMonthData);

                lblRiskScore.Text = $"{(prediction.Probability * 100):N1}%";

                // Determine risk level and apply CSS class
                if (prediction.Prediction && prediction.Probability > 0.5)
                {
                    pnlRiskPrediction.CssClass = "card";
                    lblRiskStatus.Text = "⚠️ High Risk";
                    // Add inline style for high risk
                    pnlRiskPrediction.Attributes["style"] = "background: linear-gradient(135deg, #FEE2E2, #FECACA); border: 2px solid #EF4444;";
                }
                else
                {
                    pnlRiskPrediction.CssClass = "card";
                    lblRiskStatus.Text = "✅ Low Risk";
                    // Add inline style for low risk
                    pnlRiskPrediction.Attributes["style"] = "background: linear-gradient(135deg, #D1FAE5, #A7F3D0); border: 2px solid #10B981;";
                }

                // Generate risk-based tips
                if (prediction.Prediction && prediction.Probability > 0.8)
                {
                    lblRiskRecommendation.Text = "🚨 URGENT: You're very likely to overspend! Cut unnecessary expenses immediately and avoid shopping this week.";
                }
                else if (prediction.Prediction && prediction.Probability > 0.6)
                {
                    lblRiskRecommendation.Text = "⚠️ Warning: High overspend risk detected. Review top 3 spending categories and reduce by 20% each.";
                }
                else if (prediction.Probability > 0.4)
                {
                    lblRiskRecommendation.Text = "📊 Moderate risk. Stay alert - track expenses daily and avoid impulse purchases.";
                }
                else if (prediction.Probability < 0.2)
                {
                    lblRiskRecommendation.Text = "🌟 Excellent! You're well within budget. Consider investing surplus or building emergency fund.";
                }
                else
                {
                    lblRiskRecommendation.Text = "✅ You're on track! Maintain current spending habits and continue monitoring regularly.";
                }

                pnlRiskPrediction.Visible = true;
            }
            catch (Exception ex)
            {
                lblMLStatus.Text += $" | Risk Prediction Error: {ex.Message}";
                pnlRiskPrediction.Visible = false;
            }
        }

        private List<MonthlyData> GetMonthlyData(int userId)
        {
            List<MonthlyData> data = new List<MonthlyData>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        YEAR(c.CreatedDate) as Year,
                        MONTH(c.CreatedDate) as Month,
                        ISNULL((SELECT SUM(Amount) FROM Income WHERE UserID = @UserID 
                                AND MONTH(DateReceived) = MONTH(c.CreatedDate) 
                                AND YEAR(DateReceived) = YEAR(c.CreatedDate)), 0) as Income,
                        SUM(c.Amount) as Expenses,
                        COUNT(DISTINCT c.CategoryName) as CategoryCount,
                        COUNT(*) as TransactionCount
                    FROM Categories c
                    WHERE c.UserID = @UserID
                        AND c.CreatedDate >= DATEADD(MONTH, -6, GETDATE())
                    GROUP BY YEAR(c.CreatedDate), MONTH(c.CreatedDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    float income = Convert.ToSingle(reader["Income"]);
                    float expenses = Convert.ToSingle(reader["Expenses"]);

                    data.Add(new MonthlyData
                    {
                        Income = income,
                        Expenses = expenses,
                        CategoryCount = Convert.ToInt32(reader["CategoryCount"]),
                        TransactionCount = Convert.ToInt32(reader["TransactionCount"]),
                        IsOverspending = expenses > income
                    });
                }
                reader.Close();
            }

            return data;
        }

        private MonthlyData GetCurrentMonthData(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        ISNULL((SELECT SUM(Amount) FROM Income 
                                WHERE UserID = @UserID 
                                AND MONTH(DateReceived) = MONTH(GETDATE())
                                AND YEAR(DateReceived) = YEAR(GETDATE())), 0) as Income,
                        ISNULL(SUM(c.Amount), 0) as Expenses,
                        COUNT(DISTINCT c.CategoryName) as CategoryCount,
                        COUNT(*) as TransactionCount
                    FROM Categories c
                    WHERE c.UserID = @UserID
                        AND MONTH(c.CreatedDate) = MONTH(GETDATE())
                        AND YEAR(c.CreatedDate) = YEAR(GETDATE())";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                MonthlyData data = new MonthlyData();
                if (reader.Read())
                {
                    data.Income = Convert.ToSingle(reader["Income"]);
                    data.Expenses = Convert.ToSingle(reader["Expenses"]);
                    data.CategoryCount = Convert.ToInt32(reader["CategoryCount"]);
                    data.TransactionCount = Convert.ToInt32(reader["TransactionCount"]);
                }
                reader.Close();

                return data;
            }
        }
        #endregion

        protected void btnRetrain_Click(object sender, EventArgs e)
        {
            LoadMLPredictions();
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('✅ Predictions updated successfully with latest data!');", true);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        #region ML Data Models

        public class ExpenseData
        {
            public DateTime Date { get; set; }
            public float Amount { get; set; }
            public int TransactionCount { get; set; }
        }

        public class ForecastResult
        {
            public float[] ForecastedExpenses { get; set; }
            public float[] LowerBounds { get; set; }
            public float[] UpperBounds { get; set; }
        }

        public class AnomalyData
        {
            public DateTime Date { get; set; }
            public float Amount { get; set; }
            public double Score { get; set; }
            public string Type { get; set; }
        }

        public class MonthlyTrendData
        {
            public string MonthName { get; set; }
            public float Amount { get; set; }
            public float Percentage { get; set; }
        }

        public class CategoryFeatures
        {
            public string CategoryName { get; set; }
            public float TotalSpent { get; set; }
            public int TransactionCount { get; set; }
            public float AverageAmount { get; set; }
            public float Frequency { get; set; }
        }

        public class ClusterPrediction
        {
            [ColumnName("PredictedLabel")]
            public uint PredictedClusterId { get; set; }
        }

        public class ClusterGroup
        {
            public int ClusterId { get; set; }
            public string ClusterName { get; set; }
            public string Categories { get; set; }
            public float TotalSpending { get; set; }
            public string Insight { get; set; }
        }

        public class MonthlyData
        {
            public float Income { get; set; }
            public float Expenses { get; set; }
            public int CategoryCount { get; set; }
            public int TransactionCount { get; set; }
            public bool IsOverspending { get; set; }
        }

        public class OverspendPrediction
        {
            [ColumnName("PredictedLabel")]
            public bool Prediction { get; set; }

            public float Probability { get; set; }

            public float Score { get; set; }
        }

        #endregion
    }
}