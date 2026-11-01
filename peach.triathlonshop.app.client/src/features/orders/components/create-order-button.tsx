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
import type {
  CreateOrderFormData,
  CreateOrderRequestDto,
  CreateOrderRowFormData,
} from "../data/schema";
import { useCreateOrder } from "../hooks/use-orders";
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
  const [quantity, setQuantity] = useState("1");
  const [orderArticles, setOrderArticles] = useState<
    CreateOrderRowFormData[]
  >([]);
  const createOrderMutation = useCreateOrder();

  const customerSearchTerm = customerSearch.trim();
  const articleSearchTerm = articleSearch.trim();
  const parsedQuantity = Number(quantity);
  const quantitiesAreValid = orderArticles.every((article) => {
    const parsed = Number(article.quantity);
    return Number.isInteger(parsed) && parsed > 0;
  });

  const canCreateOrder =
    selectedCustomerId !== null &&
    orderArticles.length > 0 &&
    quantitiesAreValid &&
    !createOrderMutation.isPending;
  const canAddArticle =
    selectedArticleId !== null &&
    Number.isInteger(parsedQuantity) &&
    parsedQuantity > 0;

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
      pageSize: 100,
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
              Select a customer and add one or more articles to create an order.
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
            <div className="flex items-end gap-2">
              <div className="flex-1 space-y-2">
                <label htmlFor="order-quantity">Quantity</label>
                <Input
                  id="order-quantity"
                  type="number"
                  min="1"
                  placeholder="1"
                  value={quantity}
                  onChange={(event) => setQuantity(event.target.value)}
                />
              </div>
              <Button
                type="button"
                variant="outline"
                disabled={!canAddArticle}
                onClick={() => {
                  if (selectedArticleId === null || !canAddArticle) return;

                  const selectedArticle = articles?.data.find(
                    (article) => article.id === selectedArticleId,
                  );
                  if (!selectedArticle) return;

                  setOrderArticles((current) => {
                    const existing = current.find(
                      (article) => article.articleId === selectedArticle.id,
                    );
                    if (existing) {
                      return current.map((article) =>
                        article.articleId === selectedArticle.id
                          ? {
                              ...article,
                              quantity: String(
                                Number(article.quantity) + parsedQuantity,
                              ),
                            }
                          : article,
                      );
                    }

                    return [
                      ...current,
                      {
                        articleId: selectedArticle.id,
                        articleName: selectedArticle.articleName,
                        quantity: String(parsedQuantity),
                      },
                    ];
                  });
                  setArticleSearch("");
                  setSelectedArticleId(null);
                  setQuantity("1");
                }}
              >
                Add article
              </Button>
            </div>
            {orderArticles.length > 0 && (
              <div className="space-y-2" aria-label="Articles in this order">
                <p className="text-sm font-medium">Articles in this order</p>
                {orderArticles.map((article) => (
                  <div
                    key={article.articleId}
                    className="flex items-center gap-2"
                  >
                    <span className="min-w-0 flex-1 truncate">
                      {article.articleName}
                    </span>
                    <Input
                      aria-label={`Quantity for ${article.articleName}`}
                      className="w-20"
                      type="number"
                      min="1"
                      value={article.quantity}
                      onChange={(event) =>
                        setOrderArticles((current) =>
                          current.map((item) =>
                            item.articleId === article.articleId
                              ? { ...item, quantity: event.target.value }
                              : item,
                          ),
                        )
                      }
                    />
                    <Button
                      type="button"
                      variant="outline"
                      aria-label={`Remove ${article.articleName}`}
                      onClick={() =>
                        setOrderArticles((current) =>
                          current.filter(
                            (item) => item.articleId !== article.articleId,
                          ),
                        )
                      }
                    >
                      Remove
                    </Button>
                  </div>
                ))}
              </div>
            )}
          </div>
          {createOrderMutation.error instanceof Error && (
            <p className="text-sm text-destructive" role="alert">
              {createOrderMutation.error.message}
            </p>
          )}
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setOpen(false)}
              disabled={createOrderMutation.isPending}
            >
              Close
            </Button>
            <Button
              disabled={!canCreateOrder}
              onClick={() => {
                if (selectedCustomerId === null || !canCreateOrder) {
                  return;
                }

                const formData: CreateOrderFormData = {
                  customerId: selectedCustomerId,
                  orderRows: orderArticles,
                };
                const orderData: CreateOrderRequestDto = {
                  customerId: formData.customerId,
                  orderRows: formData.orderRows.map((article) => ({
                    articleId: article.articleId,
                    quantity: Number(article.quantity),
                  })),
                };

                createOrderMutation.mutate(
                  orderData,
                  {
                    onSuccess: () => {
                      setOpen(false);
                      setCustomerSearch("");
                      setSelectedCustomerId(null);
                      setArticleSearch("");
                      setSelectedArticleId(null);
                      setQuantity("1");
                      setOrderArticles([]);
                    },
                  },
                );
              }}
            >
              {createOrderMutation.isPending ? "Creating..." : "Create"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}
