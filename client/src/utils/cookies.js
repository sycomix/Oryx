const getExpirationString = (expirationDays) => {
    const date = new Date();
    date.setTime(date.getTime() + (expirationDays*24*60*60*1000));
    return `expires=${date.toUTCString()}`;
}

export function createCookie(name, value, days) {
    let cookieString =  `${name}=${value}; path=/;`;
    
    if (days) {
        cookieString += getExpirationString(days);
    }
    
    document.cookie = cookieString;
}

export function readCookie(name) {
    const cookieString = document.cookie;
    const cookieStringSplitted = cookieString.split(`;`);

    if(cookieStringSplitted[0] !== '') {
        const foundCookie = cookieStringSplitted
                                .find(cookie => cookie.trim().startsWith(name, 0));
        
        return (foundCookie) ? foundCookie.split('=')[1] : null;
        
    }
    
    return null;
}