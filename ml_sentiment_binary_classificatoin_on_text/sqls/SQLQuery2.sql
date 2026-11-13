-- =====================================================
-- AMAZON / PRODUCT REVIEWS (POSITIVE)
-- =====================================================

INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'This vacuum cleaner exceeded my expectations and picks up pet hair effortlessly.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The battery life on this laptop is incredible and easily lasts all day.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The assembly instructions were clear and the desk feels very sturdy.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Excellent sound quality and comfortable fit even after several hours.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The coffee maker brews quickly and consistently every morning.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'This phone case fits perfectly and offers great protection.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The picture quality on this television is absolutely stunning.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The blender is powerful and handles frozen fruit with ease.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Great value for the price and arrived exactly as described.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The smartwatch setup was simple and works flawlessly.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The mattress is extremely comfortable and improved my sleep quality.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The packaging was secure and delivery was very fast.', 1);

-- AMAZON / PRODUCT REVIEWS (NEGATIVE)

INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The product stopped working after only three days of use.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The item arrived damaged and missing several parts.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The battery barely lasts two hours despite the advertised claims.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The material feels cheap and poorly made.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The headphones produce constant static noise.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The instructions were confusing and incomplete.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'This product is overpriced for the quality provided.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The color looks completely different from the photos.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The zipper broke on the first day I used it.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The software included with the device is buggy and unreliable.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The chair became uncomfortable after only thirty minutes.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The product failed to meet even basic expectations.', 0);

-- =====================================================
-- HEALTHCARE FEEDBACK
-- =====================================================

INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The doctor listened carefully and answered all my questions.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The nursing staff was kind and attentive during my stay.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'My appointment started on time and the process was smooth.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The physician explained the treatment plan clearly and professionally.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The clinic was clean, organized, and welcoming.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The test results were delivered faster than expected.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'I received excellent care during my hospital visit.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The front desk staff was friendly and efficient.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The specialist took time to explain all available options.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The telehealth appointment was convenient and easy to use.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The entire healthcare team made me feel comfortable and respected.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The facility maintained high standards of cleanliness.', 1);

INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'I waited over two hours before seeing a doctor.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The staff seemed rushed and did not answer my concerns.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'My appointment was cancelled without prior notice.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The billing process was confusing and inaccurate.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The clinic was overcrowded and poorly managed.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Nobody returned my phone calls regarding test results.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The nurse seemed distracted and inattentive.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The waiting area was unclean and uncomfortable.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'I felt rushed throughout the consultation.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The prescription information was never clearly explained.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The online patient portal frequently fails to load.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The experience did not meet my expectations for patient care.', 0);

-- =====================================================
-- INSURANCE MEMBER FEEDBACK
-- =====================================================

INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The claims process was simple and reimbursement was received quickly.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The customer service representative explained my benefits clearly.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The mobile app made it easy to track my claims.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'I found the provider directory easy to navigate.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The authorization process was faster than expected.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'My issue was resolved during the first phone call.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The explanation of benefits was detailed and easy to understand.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The support representative was knowledgeable and professional.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'I appreciated the proactive communication regarding my claim.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Finding an in-network provider was very straightforward.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The online portal offers an excellent member experience.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Overall, I am very satisfied with my insurance coverage.', 1);

INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The claim was denied without a clear explanation.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'I spent hours trying to get authorization approved.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The member portal keeps showing incorrect information.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The hold times are far too long when calling support.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The reimbursement amount was much less than expected.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The representative provided conflicting answers.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The website makes it difficult to locate important documents.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'My prior authorization request took several weeks.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The claims status information is often outdated.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The process for resolving billing issues is frustrating.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Customer support failed to resolve my concern.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'My overall experience with the insurance plan has been disappointing.', 0);

-- =====================================================
-- APP REVIEWS
-- =====================================================

INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The app is intuitive, fast, and easy to navigate.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The latest update significantly improved performance.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The user interface is clean and modern.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'This app saves me a lot of time every day.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The onboarding experience was quick and straightforward.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Notifications work perfectly and keep me informed.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The new features are genuinely useful and well designed.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The synchronization between devices is seamless.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'I use this application daily and rarely encounter issues.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The developers clearly listen to user feedback.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The app performs reliably even with heavy usage.', 1);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'This is one of the best applications I have installed.', 1);

INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The app crashes every time I try to upload a file.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The latest update introduced several annoying bugs.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The interface is cluttered and difficult to understand.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The application is much slower than previous versions.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Notifications stopped working after the recent release.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The login process fails repeatedly without explanation.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The app consumes too much battery during normal use.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Synchronization issues cause data inconsistencies.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The subscription price is too high for the features offered.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'Customer support never responded to my bug report.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'The search functionality rarely returns relevant results.', 0);
INSERT INTO dbo.SentimentData (SentimentText, Sentiment) VALUES (N'This app has become increasingly unreliable over time.', 0);



Select Count(*) from SentimentData;