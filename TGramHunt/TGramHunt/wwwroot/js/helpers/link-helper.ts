class LinkHelper {
    public notFoundPage() {
        return "/notfound";
    }

    public mainPage() {
        return "/";
    }

    public productsNew(tag: string) {
        return "/products/new?tag=" + tag;
    }

    public editProfileCheckAccess() {
        return "/editprofile/index?handler=checkaccess";
    }

    public editProfileCloseaccount() {
        return "/editprofile/index?handler=closeaccount";
    }
}

export { LinkHelper };
