import { useState } from "react";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { useCustomerSearch } from "@/features/customers/hooks/use-customers";
import { useArticlesInStock } from "../hooks/use-stock";

export function CreateOrderButton() {
  const [open, setOpen] = useState(false);
  const [customerSearch, setCustomerSearch] = useState("");
  const [selectedCustomerId, setSelectedCustomerId] = useState<number | null>(
    null,
  );
  const [articleSearch, setArticleSearch] = useState("");
  const [selectedArticleId, setSelectedArticleId] = useState<number | null>(
    null,
  );
  const customerSearchTerm = customerSearch.trim();
  const articleSearchTerm = articleSearch.trim();

  const {
    data: customers,
    error: customerError,
    isFetching: isSearchingCustomers,
  } = useCustomerSearch(
    customerSearchTerm,
    open && customerSearchTerm.length > 0,
  );
  const {
    data: articles,
    error: articleError,
    isFetching: isSearchingArticles,
  } = useArticlesInStock(
    { 
      articleName: articleSearchTerm, 
      pageSize: 100 
    },
    open && articleSearchTerm.length > 0,
  );

  return (
    <>
      <Button className="space-x-1" onClick={() => setOpen(true)}>
        <span>Create order</span> <Plus size={18} />
      </Button>
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader className="text-start">
            <DialogTitle>Create new order</DialogTitle>
            <DialogDescription>
              This form is a preview and does not save orders yet.
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <label htmlFor="order-customer">Customer</label>
              <Input
                id="order-customer"
                value={customerSearch}
                placeholder="Search customers by name"
                onChange={(event) => {
                  setCustomerSearch(event.target.value);
                  setSelectedCustomerId(null);
                }}
              />
              {customerSearchTerm && selectedCustomerId === null && (
                <div
                  className="max-h-40 overflow-y-auto rounded-md border"
                  role="listbox"
                  aria-label="Matching customers"
                >
                  {isSearchingCustomers ? (
                    <p className="p-3 text-sm text-muted-foreground">
                      Searching customers...
                    </p>
                  ) : customerError instanceof Error ? (
                    <p className="p-3 text-sm text-destructive" role="alert">
                      {customerError.message}
                    </p>
                  ) : customers?.data.length ? (
                    customers.data.map((customer) => (
                      <button
                        key={customer.id}
                        type="button"
                        role="option"
                        aria-selected={selectedCustomerId === customer.id}
                        className="block w-full px-3 py-2 text-left text-sm hover:bg-muted"
                        onClick={() => {
                          setSelectedCustomerId(customer.id);
                          setCustomerSearch(customer.name ?? "");
                        }}
                      >
                        {customer.name}
                      </button>
                    ))
                  ) : (
                    <p className="p-3 text-sm text-muted-foreground">
                      No matching customers.
                    </p>
                  )}
                </div>
              )}
            </div>
            <div className="space-y-2">
              <label htmlFor="order-article">Article</label>
              <Input
                id="order-article"
                value={articleSearch}
                placeholder="Search articles by name"
                onChange={(event) => {
                  setArticleSearch(event.target.value);
                  setSelectedArticleId(null);
                }}
              />
              {articleSearchTerm && selectedArticleId === null && (
                <div
                  className="max-h-40 overflow-y-auto rounded-md border"
                  role="listbox"
                  aria-label="Matching articles"
                >
                  {isSearchingArticles ? (
                    <p className="p-3 text-sm text-muted-foreground">
                      Searching articles...
                    </p>
                  ) : articleError instanceof Error ? (
                    <p className="p-3 text-sm text-destructive" role="alert">
                      {articleError.message}
                    </p>
                  ) : articles?.data.length ? (
                    articles.data.map((article) => (
                      <button
                        key={article.id}
                        type="button"
                        role="option"
                        aria-selected={selectedArticleId === article.id}
                        className="block w-full px-3 py-2 text-left text-sm hover:bg-muted"
                        onClick={() => {
                          setSelectedArticleId(article.id);
                          setArticleSearch(article.articleName);
                        }}
                      >
                        {article.articleName}
                      </button>
                    ))
                  ) : (
                    <p className="p-3 text-sm text-muted-foreground">
                      No matching articles.
                    </p>
                  )}
                </div>
              )}
            </div>
            <div className="space-y-2">
              <label htmlFor="order-quantity">Quantity</label>
              <Input
                id="order-quantity"
                type="number"
                min="1"
                placeholder="1"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setOpen(false)}>
              Close
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}
