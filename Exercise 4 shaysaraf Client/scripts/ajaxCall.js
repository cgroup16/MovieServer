$.ajax({
    type: method,           // סוג הבקשה (למשל, POST או GET)
    url: api,               // ה-API שאליו נשלחת הבקשה
    data: data,             // נתונים שנשלחים ב-body של הבקשה (למשל, JSON עם פרטי המוצר)
    cache: false,           // מבטל את השימוש במטמון של הדפדפן (כדי לא לשמור את התשובות בבקשות דומות)
    contentType: "application/json", // אומר לשרת שהנתונים הם בפורמט JSON
    dataType: "json",       // אומר לקוד שצפוי לקבל תשובה בפורמט JSON
    success: successCB,     // פונקציה שתופעל אם הבקשה הצליחה
    error: errorCB          // פונקציה שתופעל אם הייתה שגיאה בבקשה
});
